<template>
  <div>
    <h2>Devices</h2>
    <button @click="load">Reload</button>
    <ul>
      <li v-for="d in devices" :key="d.serialNumber">
        <strong>{{ d.serialNumber }}</strong> — {{ d.model }} @ {{ d.ipAddress }}
        <button @click="syncUser(d.serialNumber)">Sync Sample User</button>
      </li>
    </ul>
  </div>
</template>

<script>
import axios from 'axios'
export default {
  data() { return { devices: [] } },
  methods: {
    async load() {
      const res = await axios.get('/api/devices')
      this.devices = res.data
    },
    async syncUser(sn) {
      const payload = { employeeCode: 'E001', name: 'John Doe', password: '', group: '1' }
      await axios.post(`/api/devices/${sn}/sync-employee`, payload)
      alert('Queued sync')
    }
  },
  mounted() { this.load() }
}
</script>
