// src/index.ts

import Vue from 'vue'
import Component from 'vue-class-component'
import VueRouter from 'vue-router'
import BootstrapVue from 'bootstrap-vue'
import { MainPage } from './components/main.component'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'

Vue.use(BootstrapVue);
Vue.use(VueRouter);

const routes = [
    { path: '/', component: MainPage }
  ];

  const router = new VueRouter({ routes });

  new Vue({
    router: router
  }).$mount('#app');