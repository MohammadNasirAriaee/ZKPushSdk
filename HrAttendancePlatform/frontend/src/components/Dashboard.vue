<template>
  <div>
    <h2>Dashboard</h2>
    <p>Quick overview</p>
    <div>
      <button @click="refresh">Refresh</button>
    </div>
    <pre>{{ summary }}</pre>
  </div>
</template>

<script>
import axios from 'axios'
export default {
  data() {
    return { summary: null }
  },
  methods: {
    async refresh() {
      try {
        const res = await axios.get('/api/devices')
        this.summary = res.data
      } catch (e) {
        this.summary = { error: e.message }
      }
    }
  },
  mounted() {
    this.refresh()
  }
}
</script>
