<template>
  <div class="hello">
    <h1>"List of registred users"</h1>
    <div>
        {{users}}
    </div>
  </div>
</template>

<script>

const axios = require('axios')
const api = 'https://localhost:44390/api/Users'

export default {
  name: 'users',
  data () {
    return {
      msg: 'Hello from users',
      users: ''
    }
  },
  mounted: function () {
    this.loadData()
  },
  methods: {
    loadData: function () {
      let app = this
      axios({
        method: 'get',
        url: api
      })
        .then(function (response) {
          app.users = response.data
        })
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style>
h1, h2 {
  font-weight: normal;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
  display: inline-block;
  margin: 0 10px;
}

a {
  color: #35495E;
}
</style>
