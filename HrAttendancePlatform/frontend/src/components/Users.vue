<template>
  <div>
    <h2>Users</h2>
    <button @click="load">Reload</button>
    <ul>
      <li v-for="u in users" :key="u.employeeCode">{{ u.employeeCode }} — {{ u.name }}</li>
    </ul>
  </div>
</template>

<script>
import axios from 'axios'
export default {
  data() { return { users: [] } },
  methods: {
    async load() { const res = await axios.get('/api/users'); this.users = res.data }
  },
  mounted() { this.load() }
}
</script>
