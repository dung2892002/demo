<template>
  <div class="form__content" style="width: 280px; margin: 15% 40%">
    <h2>Đăng nhập</h2>
    <span v-if="errorMessage">{{ errorMessage }}</span>
    <div>
      <div class="form-group">
        <div class="form__item form__item--1">
          <span class="form__label">Username <span class="required">*</span></span>
          <input type="text" v-model="loginModel.username" />
        </div>
      </div>
      <div class="form-group">
        <div class="form__item form__item--1">
          <span class="form__label">Mật khẩu <span class="required">*</span></span>
          <input type="password" v-model="loginModel.password" />
        </div>
      </div>
      <div>
        <button @click="login" class="button--complete" v-loading="loading">Đăng nhập</button>
      </div>
      <div
        style="
          display: flex;
          flex-direction: row;
          align-items: center;
          justify-content: center;
          margin-top: 10px;
        "
      >
        <img
          src="../assets/icon/google.png"
          alt="gg"
          style="width: 24px; height: 24px; cursor: pointer"
          @click="loginGoogle"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { LoginModel } from '@/entities/LoginModel'
import '../styles/layout/form.scss'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from 'vuex'
import { startConnection } from '@/ts/signalR'
const store = useStore()
const errorMessage = ref<string | null>(null)
const router = useRouter()

const loading = ref(false)

const loginModel = ref<LoginModel>({
  username: '',
  password: '',
})

function loginGoogle() {
  const backendLoginUrl = 'https://localhost:7160/api/v1/Auths/login-google'
  window.location.href = backendLoginUrl
}

async function login() {
  loading.value = true
  const response = await store.dispatch('login', loginModel.value)
  loading.value = false
  if (response) {
    console.log('Login success')
    console.log('Start connection signalR')
    await startConnection()
    router.push({
      name: 'home',
    })
  } else {
    errorMessage.value = 'Đăng nhập thất bại, kiểm tra thông tin đăng nhập'
  }
}
</script>
