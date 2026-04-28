<template>
  <div>
    <h2>Attendance</h2>
    <button @click="load">Reload</button>
    <ul>
      <li v-for="p in punches" :key="p.id">{{ p.employeeCode }} — {{ p.time }}</li>
    </ul>
  </div>
</template>

<script>
import axios from 'axios'
export default {
  data() { return { punches: [] } },
  methods: {
    async load() { const res = await axios.get('/api/attendance/punches'); this.punches = res.data }
  },
  mounted() { this.load(); this.timer = setInterval(this.load, 3000) },
  beforeUnmount() { clearInterval(this.timer) }
}
</script>
