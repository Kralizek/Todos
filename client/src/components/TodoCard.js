import React from 'react';
import '../styles/TodoCard.css';

function TodoCard({ todo,  onDelete }) {
  return (
    <div className="todo-card">
      <div className="todo-details">
        <h3>{todo.title}</h3>
        <p>{todo.description}</p>
        <p className='priority'>Priority: {todo.priority}</p>
        
      </div>
      <div className="todo-actions">
        <button onClick={() => onDelete(todo.id)}>Delete</button>
      </div>
    </div>
  );
}

export default TodoCard;