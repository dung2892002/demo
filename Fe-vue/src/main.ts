import '../src/styles/main.scss'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { store } from './store'
import vLoading from './directives/vLoading'
const app = createApp(App)

app.use(router)
app.use(store)
app.directive('loading', vLoading)

app.mount('#app')
