import React, { useState, useEffect } from 'react';
import '../styles/AddTodo.css';

function AddTodo({ todo, onSave}) {
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    priority:0,
    isComplete: true,
  });

  useEffect(() => {
    if (todo) {
      setFormData({
        title: todo.title,
        description: todo.description,
        priority: todo.priority,
        isComplete: true,
      });
    }
  }, [todo]);

  const handleChange = (e) => {
    let val
    const { name, value } = e.target;
    (e.target.type == "number") ? val = Number(value) : val = value;
    setFormData((prevData) => ({
      ...prevData,
      [name]: val,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    await onSave(formData);

  };

  return (
    <div className="add-todo">
      <h2>{'Add Todo'}</h2>
      <form onSubmit={handleSubmit}>
        <label>
          Title:
          <input
            type="text"
            name="title"
            value={formData.title}
            onChange={handleChange}
          />
        </label>
        <label>
          Description:
          <textarea
            name="description"
            value={formData.description}
            onChange={handleChange}
          />
        </label>
        <label>
        Priority:
          <input
            type="number"
            name="priority"
            value={formData.priority}
            onChange={handleChange}
          />
        </label>

        <div className="buttons">
          <button type="submit">{'Add'}</button>
        </div>
      </form>
    </div>
  );
}

export default AddTodo;
