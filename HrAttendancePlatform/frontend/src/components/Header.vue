<template>
  <div class="app-container flex items-center justify-between h-16">
    <div class="flex items-center gap-4">
      <div class="text-2xl font-semibold text-slate-800">HR Attendance</div>
      <span class="text-sm text-slate-500">Management Dashboard</span>
    </div>

    <div class="flex items-center gap-4">
      <div class="hidden sm:block">
        <input
          class="focus-ring rounded-md border px-3 py-1 bg-white text-sm shadow-sm"
          placeholder="Search devices, users..."
          aria-label="Search"
        />
      </div>

      <button class="flex items-center gap-2 p-1 rounded-md hover:bg-slate-100 focus-ring">
        <img src="/avatar.png" alt="user" class="w-8 h-8 rounded-full bg-slate-200" />
        <span class="hidden sm:inline text-sm">Admin</span>
      </button>
    </div>
  </div>
</template>

<script>
export default { name: 'Header' }
</script>

<style scoped>
/* local refinements if needed */
</style>
