import React from 'react';
import TodoCard from './TodoCard';
import '../styles/TodoList.css';

function TodoList({ todos, onDelete }) {
    const sortedTodos = todos.slice().sort((a,b)=> a.priority - b.priority)
    return (
        <div className="todo-list">
        {sortedTodos.map((todo) => (
            <TodoCard
            key={todo.id}
            todo={todo}
            onDelete={onDelete}
            />
            ))}
      </div>
    );
}

export default TodoList;