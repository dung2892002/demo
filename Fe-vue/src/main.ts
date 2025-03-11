import '../src/styles/main.scss'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { store } from './store'
import vLoading from './directives/vLoading'

import { library } from '@fortawesome/fontawesome-svg-core'

import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

import { fas } from '@fortawesome/free-solid-svg-icons'
import { far } from '@fortawesome/free-regular-svg-icons'
import vDraggable from './directives/vDraggable'
library.add(fas, far)

const app = createApp(App)
app.component('font-awesome-icon', FontAwesomeIcon)
app.use(router)
app.use(store)
app.directive('loading', vLoading)
app.directive('draggable', vDraggable)

app.mount('#app')
