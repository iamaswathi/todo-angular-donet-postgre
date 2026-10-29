import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Todo, TodoService } from './todo.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule],
  template: `
    <main>
      <h1>Todo List</h1>

      <form (ngSubmit)="add()">
        <input name="title" [(ngModel)]="newTitle" placeholder="New Tasks to be done?" autocomplete="off" />
        <button type="submit" [disabled]="!newTitle.trim()">Add</button>
      </form>

      @if (error()) { <p class="error">{{ error() }}</p> }

      <ul>
        @for (t of todos(); track t.id) {
          <li [class.done]="t.isDone">
            <input type="checkbox" [checked]="t.isDone" (change)="toggle(t)" />
            <span>{{ t.title }}</span>
            <button class="del" (click)="remove(t)" aria-label="Delete">✕</button>
          </li>
        } @empty {
          <li class="empty">Nothing to do. Add something above.</li>
        }
      </ul>
    </main>
  `,
  styles: [`
    main { max-width: 480px; margin: 3rem auto; font-family: system-ui, sans-serif; padding: 0 1rem; }
    form { display: flex; gap: .5rem; margin-bottom: 1rem; }
    form input { flex: 1; padding: .6rem; font-size: 1rem; }
    button { padding: .6rem 1rem; font-size: 1rem; cursor: pointer; }
    ul { list-style: none; padding: 0; }
    li { display: flex; align-items: center; gap: .75rem; padding: .6rem 0; border-bottom: 1px solid #ddd; }
    li span { flex: 1; }
    li.done span { text-decoration: line-through; color: #888; }
    .del { padding: .2rem .5rem; background: none; border: none; color: #c00; }
    .empty { color: #888; }
    .error { color: #c00; }
  `],
})
export class AppComponent implements OnInit {
  private api = inject(TodoService);

  todos = signal<Todo[]>([]);
  error = signal('');
  newTitle = '';

  ngOnInit() { this.load(); }

  load() {
    this.api.list().subscribe({
      next: (items) => { this.todos.set(items); this.error.set(''); },
      error: () => this.error.set('Cannot reach the API. Is the backend running on port 5080?'),
    });
  }

  add() {
    const title = this.newTitle.trim();
    if (!title) return;
    this.api.add(title).subscribe({
      next: (t) => { this.todos.update((list) => [...list, t]); this.newTitle = ''; },
      error: () => this.error.set('Failed to add todo.'),
    });
  }

  toggle(t: Todo) {
    const updated = { ...t, isDone: !t.isDone };
    this.api.update(updated).subscribe({
      next: (res) => this.todos.update((list) => list.map((x) => (x.id === res.id ? res : x))),
      error: () => this.error.set('Failed to update todo.'),
    });
  }

  remove(t: Todo) {
    this.api.remove(t.id).subscribe({
      next: () => this.todos.update((list) => list.filter((x) => x.id !== t.id)),
      error: () => this.error.set('Failed to delete todo.'),
    });
  }
}
