import '../src/styles/main.scss'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { store } from './store'
import vLoading from './directives/vLoading'
import vue3GoogleLogin from 'vue3-google-login'

import { library } from '@fortawesome/fontawesome-svg-core'

import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

import { fas } from '@fortawesome/free-solid-svg-icons'
library.add(fas)

const app = createApp(App)
app.use(vue3GoogleLogin, {
  clientId: 'YOUR_GOOGLE_CLIENT_ID',
})
app.component('font-awesome-icon', FontAwesomeIcon)
app.use(router)
app.use(store)
app.directive('loading', vLoading)

app.mount('#app')
