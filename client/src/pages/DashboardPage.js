import React, { useState, useEffect } from 'react';
import TodoList from '../components/TodoList';
import AddTodo from '../components/AddTodo'; 
import { getAllTodos, createTodo, updateTodo, deleteTodo } from '../services/api'; 

function DashboardPage() {
  const [todos, setTodos] = useState([]);
  const [addingTodo, setAddingTodo] = useState(null);

  useEffect(() => {
    fetchTodos();
  }, []);

  const fetchTodos = async () => {
    try {
      const productsData = await getAllTodos();
      setTodos(productsData);
    } catch (error) {
      console.error('Error fetching todos:', error);
    }
  };


  const handleSave = async (todoData) => {
    try {

        await createTodo(todoData);

      fetchTodos();
    } catch (error) {
      console.error('Error saving todo:', error);
    }
  };


  const handleDelete = async (todoId) => {
    try {
      await deleteTodo(todoId);
      fetchTodos();
    } catch (error) {
      console.error('Error deleting todo:', error);
    }
  };

  return (
    <div className="home-page">
      <h1>Todo List</h1>
      <AddTodo todo={addingTodo} onSave={handleSave} />
      <TodoList todos={todos} onDelete={handleDelete} />
    </div>
  );
}

export default DashboardPage;