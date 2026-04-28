<template>
  <div>
    <nav style="padding:12px;border-bottom:1px solid #ddd">
      <a href="/">Dashboard</a> | <a href="/devices">Devices</a> | <a href="/users">Users</a> | <a href="/attendance">Attendance</a>
    </nav>
    <main style="padding:16px">
      <router-view />
    </main>
  </div>
</template>

<script>
export default { name: 'App' }
</script>

<style>
body { font-family: Arial, Helvetica, sans-serif }
a { margin: 0 8px }
</style>
