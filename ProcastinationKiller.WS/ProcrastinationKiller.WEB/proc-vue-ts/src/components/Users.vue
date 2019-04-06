<template>
  <div>
    <h3>TS Counter</h3>
    <div>{{count}}</div>
    <div>
    <button @click="increment()">INCREMENT</button>
    </div>
  </div>
</template>

<script lang="ts">
import * as Vue from 'vue'
import Component from 'vue-class-component'

@Component
export default class UsersComponent extends Vue {
  // Initial data can be declared as instance properties
  message: string = 'Hello!'

  // Component methods can be declared as instance methods
  onClick (): void {
    window.alert(this.message)
  }
}

</script>