import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import App from './App.vue'
import Dashboard from './components/Dashboard.vue'
import Devices from './components/Devices.vue'
import Users from './components/Users.vue'
import Attendance from './components/Attendance.vue'
import './index.css'

const routes = [
  { path: '/', component: Dashboard },
  { path: '/devices', component: Devices },
  { path: '/users', component: Users },
  { path: '/attendance', component: Attendance }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

createApp(App).use(router).mount('#app')
