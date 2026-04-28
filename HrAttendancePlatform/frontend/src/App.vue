<template>
  <div class="min-h-screen bg-slate-50 text-slate-800">
    <header class="bg-white shadow">
      <Header />
    </header>

    <div class="flex">
      <aside class="hidden md:block w-64 bg-white border-r">
        <Sidebar />
      </aside>

      <main class="flex-1 p-6 app-container">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script>
import Header from './components/Header.vue'
import Sidebar from './components/Sidebar.vue'

export default {
  name: 'App',
  components: { Header, Sidebar }
}
</script>

<style>
/* Intentionally minimal; styling driven by Tailwind */
</style>
