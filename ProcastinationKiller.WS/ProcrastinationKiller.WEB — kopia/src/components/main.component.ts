import Vue from 'vue';
import Component from 'vue-class-component';

@Component({
  name: 'main-page',
  template: `
  <div>
    <h3>Add Todo</h3>
  </div>
  `
})

export class MainPage extends Vue {
}