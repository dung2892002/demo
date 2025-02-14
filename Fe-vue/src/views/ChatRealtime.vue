<template>
  <div class="content--row" style="width: 100%; display: flex; flex-direction: row">
    <div style="width: 20%; border-right: 1px solid black; padding: 20px 0">
      <div v-for="(account, index) in userToChat" :key="index" style="overflow: hidden">
        <div
          class="chat_user"
          :class="{ selected: account.UserId == receiverId }"
          v-if="account.UserId != getUserId()"
          @click="selectChatBox(account)"
        >
          <img src="/src/assets/img/default-avatar.jpg" alt="" />
          <div style="display: flex; flex-direction: column">
            <span style="font-size: 16px">{{ account.UserName }}</span>
            <span style="color: gray; display: flex; flex-direction: row; gap: 4px">
              <span v-if="account.Type">Bạn: </span>
              <span
                style="max-width: 160px"
                :class="{ read: account.MessageStatus == false && !account.Type }"
                >{{ account.MessageContent }}</span
              >
            </span>
          </div>

          <span
            style="
              color: white;
              background-color: red;
              width: 20px;
              height: 20px;
              font-size: 14px;
              font-weight: bold;
              border-radius: 50%;
              display: flex;
              justify-content: center;
              align-items: center;
              position: absolute;
              right: 40px;
            "
            v-if="!account.Type && account.TotalMessageUnRead > 0"
            >{{ account.TotalMessageUnRead }}</span
          >
        </div>
      </div>
    </div>
    <div v-if="accountReceiver" style="width: 80%; padding: 20px">
      <h3>{{ accountReceiver.UserName }}</h3>
      <div class="status">
        <span
          :class="{ online: isReceiverOnline(receiverId), offline: !isReceiverOnline(receiverId) }"
        >
          {{ isReceiverOnline(receiverId) ? 'Đang hoạt động' : 'Ngoại tuyến' }}
        </span>
      </div>
      <div class="messages" ref="messagesContainer">
        <span v-if="senderTyping" class="message receiver">
          Đang nhập <span class="dots"></span>
        </span>
        <span
          class="message"
          v-for="msg in messages"
          :key="msg.MessageId"
          :class="{
            sender: msg.SenderId === getUserId(),
            receiver: msg.ReceiverId === getUserId(),
          }"
        >
          {{ msg.Content }}
        </span>
        <div @click="handleGetMoreMessages" v-if="pageNumber < totalPages" class="load_more">
          <button v-loading="loading">
            <img src="/src/assets/icon/up.png" alt="them tin nhan" />
          </button>
        </div>
      </div>
      <div class="input_message">
        <input
          v-model="newMessage"
          type="text"
          placeholder="Nhập tin nhắn..."
          @input="onTyping()"
          @keydown.enter.prevent="sendMessage"
        />
        <button @click="sendMessage">
          <img src="/src/assets/icon/send.png" alt="" />
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, nextTick, onBeforeUnmount } from 'vue'
import axios from 'axios'
import { connection } from '@/signalR'
import { useStore } from 'vuex'

const store = useStore()

const newMessage = ref<string>('')
const pageNumber = ref<number>(1)

const senderTyping = ref<boolean>(false)
const receiverId = ref<string>('')
const messages = ref<Message[]>([])

const accountReceiver = ref<Account | null>(null)
const userToChat = ref<Account[]>([])

const messagesContainer = ref<HTMLElement | null>(null)

const skipItem = ref<number>(0)
const totalPages = ref<number>(1)

const loading = ref<boolean>(false)

interface Account {
  UserId: string
  UserName: string
  MessageContent: string
  MessageCreatedAt: Date
  MessageStatus: boolean
  TotalMessageUnRead: number
  Type: boolean
}

interface Message {
  MessageId: number
  SenderId: string
  ReceiverId: string
  Content: string
  Status: boolean
  CreatedAt: string
}

function getUserId(): string {
  return localStorage.getItem('userId') || ''
}

async function fetchUserToChat() {
  const response = await axios.get(`https://localhost:7160/api/v1/Messages/user`, {
    params: {
      userId: getUserId(),
    },
  })

  userToChat.value = response.data
}

const onlineUsers = computed(() => store.getters.getOnlineUsers)

function updateUserChat(
  userId: string,
  userName: string,
  message: Message,
  type: boolean,
  totalMessageUnRead: number,
) {
  const idx = userToChat.value.findIndex((m) => m.UserId === message.SenderId)

  const totalMessage =
    type === true
      ? 0
      : (userToChat.value[idx].TotalMessageUnRead + totalMessageUnRead) * totalMessageUnRead

  userToChat.value = userToChat.value.filter((u) => u.UserId !== userId)

  const newUserChat: Account = {
    UserId: userId,
    UserName: userName,
    MessageContent: message.Content,
    MessageCreatedAt: new Date(message.CreatedAt),
    MessageStatus: message.Status,
    TotalMessageUnRead: totalMessage,
    Type: type,
  }

  userToChat.value.unshift(newUserChat)
}

function scrollToBottom() {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
    }
  })
}

async function selectChatBox(account: Account) {
  accountReceiver.value = account
  receiverId.value = account.UserId
  newMessage.value = ''
  pageNumber.value = 1
  skipItem.value = 0
  messages.value = []

  totalPages.value = await fetchMessages()

  scrollToBottom()
}

function isReceiverOnline(id: string): boolean {
  return onlineUsers.value.includes(id)
}

async function fetchMessages() {
  const response = await axios.get(`https://localhost:7160/api/v1/Messages`, {
    params: {
      userId1: getUserId(),
      userId2: receiverId.value,
      pageNumber: pageNumber.value,
      skipItem: skipItem.value,
    },
  })
  const fetchMessages: Message[] = response.data.Messages
  messages.value = messages.value.concat(fetchMessages)

  const lastMessage = fetchMessages[0]
  if (lastMessage && lastMessage.Status === false) {
    ReadMessage(lastMessage)
  }

  return response.data.TotalPages
}

async function onTyping() {
  if (newMessage.value.length > 0) {
    await connection.invoke('OnTyping', receiverId.value)
  }
}

async function sendMessage() {
  senderTyping.value = false
  if (newMessage.value.trim() === '') return
  const message = {
    MessageId: Math.random(),
    SenderId: getUserId(),
    ReceiverId: receiverId.value,
    Content: newMessage.value,
    Status: false,
    CreatedAt: new Date().toISOString(),
  }

  messages.value.unshift(message)
  try {
    await connection.invoke('SendMessage', receiverId.value, newMessage.value)
    skipItem.value++
    scrollToBottom()
    if (accountReceiver.value) {
      updateUserChat(receiverId.value, accountReceiver.value?.UserName, message, true, 0)
    }
  } catch (err) {
    console.error('Lỗi khi gửi tin nhắn:', err)
    messages.value.splice(messages.value.indexOf(message), 1)
  }
  newMessage.value = ''
}

async function handleGetMoreMessages() {
  if (pageNumber.value < totalPages.value) {
    loading.value = true
    pageNumber.value++
    await fetchMessages()
    loading.value = false
  }
}

async function ReadMessage(message: Message) {
  const idx = userToChat.value.findIndex((m) => m.UserId === message.SenderId)
  if (idx !== -1) {
    await axios.patch(`https://localhost:7160/api/v1/Messages/${message.MessageId}`)
    userToChat.value[idx].MessageStatus = true
    userToChat.value[idx].TotalMessageUnRead = 0
  }
}

onMounted(async () => {
  fetchUserToChat()

  connection.on('SenderTypingMessage', (senderId: string) => {
    if (senderId === receiverId.value) {
      senderTyping.value = true
    }
  })

  connection.on('ReceiveMessage', (senderId: string, senderName: string, message: Message) => {
    if (senderId === receiverId.value) {
      messages.value.unshift(message)
      senderTyping.value = false
      message.Status = true
      ReadMessage(message)
      skipItem.value++
    }
    updateUserChat(senderId, senderName, message, false, 1)
  })
})

onBeforeUnmount(() => {
  connection.off('SenderTypingMessage')
  connection.off('ReceiveMessage')
})
</script>

<style scoped lang="scss">
.chat_user {
  cursor: pointer;
  background-color: #ffffff;
  padding-left: 10px;
  height: 50px;
  display: flex;
  flex-direction: row;
  align-items: center;
  gap: 8px;
  position: relative;

  &:hover {
    background-color: #cccbcb;
  }

  &.selected {
    background-color: #b3f9b3;
  }

  img {
    width: 24px;
    height: 24px;
    border-radius: 50%;
    border: 1px solid #b0b0b0;
  }

  span {
    font-size: 14px;
    max-width: 200px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    display: inline-block;
  }
}

.status {
  margin-bottom: 10px;
  border-bottom: 1px solid black;

  .online {
    color: green;
  }
  .offline {
    color: red;
  }
}

.messages {
  max-height: 680px;
  overflow-y: scroll;
  display: flex;
  flex-direction: column-reverse;
  align-self: self-start;

  &::-webkit-scrollbar {
    width: 8px;
  }

  &::-webkit-scrollbar-track {
    background: #f0f0f0;
    border-radius: 4px;
  }

  &::-webkit-scrollbar-thumb {
    background-color: #b0b0b0;
    border-radius: 4px;
    border: 2px solid #f0f0f0;
  }

  &::-webkit-scrollbar-thumb:hover {
    background-color: #888;
  }
}

.message {
  padding: 6px 12px;
  margin: 4px;
  border-radius: 24px;
  display: inline-block;
  width: fit-content;
  max-width: 720px;
  word-wrap: break-word;

  &.sender {
    align-self: flex-end;
    background-color: #b3f9b3;
  }

  &.receiver {
    background-color: #f1f1f1;
  }
}

.read {
  font-weight: bold;
  color: black;
}

.input_message {
  display: flex;
  flex-direction: row;
  justify-content: end;
  align-items: center;
  margin-top: 20px;

  input {
    width: 100%;
    margin: 0;

    &:focus {
      outline: 2px solid black;
    }
  }

  button {
    height: 36px;
    margin-left: 6px;
    background-color: #b3f9b3;

    img {
      width: 24px;
      height: 24px;
    }
  }
}

.load_more {
  display: flex;
  justify-content: center;
  margin-top: 10px;

  button {
    border: none;
    padding: 8px;
    cursor: pointer;

    img {
      width: 24px;
      height: 24px;
    }
  }
}

.dots::after {
  content: '';
  display: inline-block;
  animation: dotsAnimation 1.5s steps(3, end) infinite;
  font-weight: bold;
  margin-left: 5px;
}

@keyframes dotsAnimation {
  0% {
    content: '';
  }
  33% {
    content: '.';
  }
  66% {
    content: '..';
  }
  100% {
    content: '...';
  }
}
</style>
