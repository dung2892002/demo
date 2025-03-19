<template>
  <div class="document-blocks">
    <div class="block title">
      <div @click="toggleFirstBlocks" v-if="firstBlocks.length > 0" class="control">
        <font-awesome-icon :icon="['fas', 'chevron-down']" size="2xs" v-if="showFirstBlocks" />
        <font-awesome-icon :icon="['fas', 'chevron-right']" size="2xs" v-else />
      </div>
      <div v-else class="control"></div>
      <span>Mở đầu</span>
    </div>
    <div v-show="showFirstBlocks">
      <div
        class="block"
        v-for="(block, index) in firstBlocks"
        :key="index"
        :style="{ paddingLeft: block.Level != 0 ? block.Level * 20 + 'px' : '40px' }"
        v-show="block.IsShow"
      >
        <div
          class="block__level"
          :style="{ backgroundColor: colors[block.Level - 1].color }"
          v-if="block.Level != 0"
        ></div>
        <div v-html="marked(block.Content)" class="markdown-container"></div>
      </div>
    </div>
    <div class="block title">
      <div @click="toggleContentBlocks" v-if="contentBlocks.length > 0" class="control">
        <font-awesome-icon :icon="['fas', 'chevron-down']" size="2xs" v-if="showContentBlocks" />
        <font-awesome-icon :icon="['fas', 'chevron-right']" size="2xs" v-else />
      </div>
      <div v-else class="control"></div>
      <span>Nội dung</span>
    </div>
    <div v-show="showContentBlocks">
      <div
        class="block"
        v-for="(block, index) in contentBlocks"
        :key="index"
        :style="{ paddingLeft: block.Level != 0 ? block.Level * 20 + 'px' : '40px' }"
        v-show="block.IsShow == true"
      >
        <div v-if="checkHasChild(block)" @click="toggleExpandBlock(block)" class="control">
          <font-awesome-icon :icon="['fas', 'chevron-down']" size="2xs" v-if="block.IsExpand" />
          <font-awesome-icon :icon="['fas', 'chevron-right']" size="2xs" v-else />
        </div>
        <div
          class="block__level"
          :style="{ backgroundColor: colors[block.Level - 1].color }"
          v-if="block.Level != 0"
        ></div>
        <div v-html="marked(block.Content)" class="markdown-container"></div>
      </div>
    </div>
    <div class="block title">
      <div @click="toggleSignBlocks" v-if="signBlocks.length > 0" class="control">
        <font-awesome-icon :icon="['fas', 'chevron-down']" size="2xs" v-if="showSignBlocks" />
        <font-awesome-icon :icon="['fas', 'chevron-right']" size="2xs" v-else />
      </div>
      <div v-else class="control"></div>
      <span>Ký tên</span>
    </div>
    <div v-show="showSignBlocks">
      <div
        class="block"
        v-for="(block, index) in signBlocks"
        :key="index"
        :style="{ paddingLeft: block.Level != 0 ? block.Level * 20 + 'px' : '40px' }"
      >
        <div
          class="block__level"
          :style="{ backgroundColor: colors[block.Level - 1].color }"
          v-if="block.Level != 0"
        ></div>
        <div v-html="marked(block.Content)" class="markdown-container"></div>
      </div>
    </div>
    <div class="block title">
      <div @click="toggleOtherBlocks" v-if="otherBlocks.length > 0" class="control">
        <font-awesome-icon :icon="['fas', 'chevron-down']" size="2xs" v-if="showOtherBlocks" />
        <font-awesome-icon :icon="['fas', 'chevron-right']" size="2xs" v-else />
      </div>
      <div v-else></div>
      <span>Khác</span>
    </div>
    <div v-show="showOtherBlocks">
      <div
        class="block"
        v-for="(block, index) in otherBlocks"
        :key="index"
        :style="{ paddingLeft: block.Level != 0 ? block.Level * 20 + 'px' : '40px' }"
      >
        <div
          class="block__level"
          :style="{ backgroundColor: colors[block.Level - 1].color }"
          v-if="block.Level != 0"
        ></div>
        <div v-html="marked(block.Content)" class="markdown-container"></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { DocumentBlock } from '@/entities/Document'
import { marked } from 'marked'
import { onMounted, ref, watch, type PropType } from 'vue'

marked.setOptions({
  breaks: true,
  gfm: true,
})

const colors = [
  {
    color: '#e81c2b',
    name: 'Phần',
  },
  {
    color: '#f4891e',
    name: 'Chương',
  },
  {
    color: '#eace2a',
    name: 'Mục',
  },
  {
    color: '#0aa34f',
    name: 'Tiểu mục',
  },
  {
    color: '#459fe3',
    name: 'Điều',
  },
  {
    color: '#d80b8f',
    name: 'Khoản',
  },
  {
    color: '#6a3499',
    name: 'Điểm',
  },
]

function checkHasChild(block: DocumentBlock) {
  return props.blocks.some((b) => b.ParentId == block.Id)
}

function toggleExpandBlock(block: DocumentBlock) {
  if (block.IsExpand) {
    handleCollapseBlock(block)
  } else {
    handleExpandBlock(block)
  }
}

function handleCollapseBlock(block: DocumentBlock) {
  block.IsExpand = false
  hiddenBlock(block)
  block.IsShow = true
}

function handleExpandBlock(block: DocumentBlock) {
  block.IsExpand = true
  showBlock(block)
  block.IsShow = true
}

function hiddenBlock(block: DocumentBlock) {
  block.IsShow = false
  const childenBlocks = props.blocks.filter((b) => b.ParentId == block.Id)
  childenBlocks.forEach((b) => {
    hiddenBlock(b)
  })
}

function showBlock(block: DocumentBlock) {
  const childenBlocks = props.blocks.filter((b) => b.ParentId == block.Id)
  childenBlocks.forEach((b) => {
    b.IsShow = true
    if (b.IsExpand) {
      showBlock(b)
    }
  })
}

const props = defineProps({
  blocks: {
    type: Array as PropType<DocumentBlock[]>,
    required: true,
  },
})

const firstBlocks = ref<DocumentBlock[]>([])
const showFirstBlocks = ref(true)
function toggleFirstBlocks() {
  showFirstBlocks.value = !showFirstBlocks.value
}

const contentBlocks = ref<DocumentBlock[]>([])
const showContentBlocks = ref(true)
function toggleContentBlocks() {
  showContentBlocks.value = !showContentBlocks.value
}

const signBlocks = ref<DocumentBlock[]>([])
const showSignBlocks = ref(true)
function toggleSignBlocks() {
  showSignBlocks.value = !showSignBlocks.value
}

const otherBlocks = ref<DocumentBlock[]>([])
const showOtherBlocks = ref(true)
function toggleOtherBlocks() {
  showOtherBlocks.value = !showOtherBlocks.value
}

function handleSplitBlock() {
  firstBlocks.value = []
  contentBlocks.value = []
  signBlocks.value = []
  otherBlocks.value = []

  props.blocks.forEach((block) => {
    block.IsShow = true
    block.IsExpand = true
    if (block.ContentType == 1) {
      firstBlocks.value.push(block)
    } else if (block.ContentType == 2) {
      contentBlocks.value.push(block)
    } else if (block.ContentType == 4) {
      signBlocks.value.push(block)
    } else {
      otherBlocks.value.push(block)
    }
  })
}

watch(
  () => props.blocks,
  () => {
    handleSplitBlock()
  },
)

onMounted(() => {
  handleSplitBlock()
})
</script>

<style lang="scss" scoped>
.document-blocks {
  display: flex;
  flex-direction: column;
  .block {
    display: flex;
    flex-direction: row;
    align-items: flex-start;
    gap: 12px;
    border-bottom: 1px solid #d6d6d6;
    border-radius: 0;
    padding: 8px 0px;
    padding-right: 20px;
    min-height: 32px;
    cursor: pointer;

    &.title {
      gap: 40px;
    }

    .control {
      width: 12px;
      padding: 0 4px;
    }

    &:hover {
      background-color: #f5f5f5;
    }

    .block__level {
      width: 10px !important;
      height: 10px !important;
      padding: 0 !important;
      border-radius: 50% !important;
      margin-top: 7px;
      flex-shrink: 0;
    }

    .markdown-container {
      width: 100%;
    }
  }
}
</style>
