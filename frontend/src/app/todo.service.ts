import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface Todo {
  id: number;
  title: string;
  isDone: boolean;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class TodoService {
  private http = inject(HttpClient);
  private base = '/api/todos'; // proxied to the .NET API in dev (see proxy.conf.json)

  list() { return this.http.get<Todo[]>(this.base); }
  add(title: string) { return this.http.post<Todo>(this.base, { title }); }
  update(t: Todo) { return this.http.put<Todo>(`${this.base}/${t.id}`, { title: t.title, isDone: t.isDone }); }
  remove(id: number) { return this.http.delete<void>(`${this.base}/${id}`); }
}
