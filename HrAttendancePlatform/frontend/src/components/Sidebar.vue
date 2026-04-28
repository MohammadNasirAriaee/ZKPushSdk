<template>
  <nav class="p-4">
    <ul class="space-y-1">
      <li>
        <router-link to="/" class="flex items-center gap-3 p-2 rounded-md hover:bg-slate-100" active-class="bg-slate-100">
          <span class="font-medium">Dashboard</span>
        </router-link>
      </li>
      <li>
        <router-link to="/devices" class="flex items-center gap-3 p-2 rounded-md hover:bg-slate-100" active-class="bg-slate-100">
          Devices
        </router-link>
      </li>
      <li>
        <router-link to="/users" class="flex items-center gap-3 p-2 rounded-md hover:bg-slate-100" active-class="bg-slate-100">
          Users
        </router-link>
      </li>
      <li>
        <router-link to="/attendance" class="flex items-center gap-3 p-2 rounded-md hover:bg-slate-100" active-class="bg-slate-100">
          Attendance
        </router-link>
      </li>
    </ul>
  </nav>
</template>

<script>
export default { name: 'Sidebar' }
</script>

<style scoped>
nav { min-height: 100vh }
</style>
