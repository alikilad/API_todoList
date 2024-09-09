<template>
  <div class="todo-app">
    <h1>To-Do List</h1>
    
    <!-- Input for adding a new task -->
    <div class="input-group">
      <input v-model="newTaskName" class="task-input" placeholder="Enter a new task" />
      <input v-model="newTaskTag" class="task-input tag-input" placeholder="Enter a tag (optional)" />
      <input v-model="newTaskDueDate" type="date" class="task-input date-input" placeholder="Enter due date" />
      <button @click="addTask" class="add-button">Add Task</button>
    </div>

    <ul class="task-list">
      <!-- Loop through tasks and display each task with a checkbox for marking done/undone -->
      <li v-for="task in tasks" :key="task.id" class="task-item">
        <div>
          <input type="checkbox" :checked="task.done" @change="toggleTaskStatus(task)" /> <!-- Checkbox to toggle done status -->
          <span :class="{ 'task-done': task.done }">{{ task.name }}</span>

          <!-- Display tags next to each task -->
          <span v-if="task.tags.length > 0" class="task-tags">
            <span v-for="tag in task.tags" :key="tag" class="tag">{{ tag }}</span>
          </span>

          <!-- Display due date -->
          <div v-if="task.dueDate" class="task-dates">Due by: {{ task.dueDate }}</div>

          <!-- Display completed date if task is done -->
          <div v-if="task.completedDate" class="task-dates">Completed on: {{ task.completedDate }}</div>
        </div>
        <button @click="startEditingTask(task)" class="edit-button">Edit</button>
        <button @click="deleteTask(task.id)" class="delete-button">Delete</button>
      </li>
    </ul>

    <!-- Form to edit a task -->
    <div v-if="editingTask" class="edit-form"> 
    <h2>Edit Task</h2> <input v-model="editingTask.name" placeholder="Edit task name" /> 
    <input v-model="editingTask.dueDate" type="date" placeholder="Edit due date" /> 
    <button @click="saveTask(editingTask)">Save</button>
    <button @click="cancelEdit" class="cancel-button">Cancel</button> 
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'; // Import the Axios instance
// Define the reactive variables to simulate tasks with tags
const tasks = ref([]) // This will hold the tasks fetched from the API
const newTaskName = ref('')  // Name of the new task to be added
const newTaskTag = ref('')  // Tag for the new task
const newTaskDueDate = ref('') // Due date for the new task
const editingTask = ref(null);

// Fetch tasks from the backend API on component mount
onMounted(async () => {
  try {
    const response = await axios.get('/api/task')  // Use relative URL
    tasks.value = JSON.parse(response.data);
  } catch (error) {
    console.error('Error fetching tasks:', error)
  }
})

// Function to add a new task with a "done" status and tags
const addTask = async() => {
  if (newTaskName.value) {
    try{
      const newTask = {
      id: tasks.value.length + 1,
      name: newTaskName.value,
      done: false,  // New tasks start as not done
      tags: newTaskTag.value ? [newTaskTag.value] : [],  // Add tag if provided
      dueDate: newTaskDueDate.value || null, // Add due date if provided
      completedDate: null //Date initially not completed
      };
      const response = await  axios.post('/api/task', newTask);
      console.log(Array.isArray(tasks.value));
      console.log(tasks.value);
      console.log(typeof tasks.value);
      tasks.value.push(response.data)  // Add the new task to the list
      newTaskName.value = ''  // Clear the input fields after adding
      newTaskTag.value = ''  // Clear the tag field after adding
      newTaskDueDate.value = '' //Clear the due date field
    } 
  catch (error) {
    console.error('Error adding task:', error);
  }
  }
};

// Function to delete a task
const deleteTask = async(id) => {
  try {
      await axios.delete(`/api/task/${id}`)
      tasks.value = tasks.value.filter(task => task.id !== id)
  }
  catch (error) {
    console.error('Error deleting task:', error);
  }
}

// Function to toggle task completion status
const toggleTaskStatus = async(task) => {
  task.done = !task.done  // Toggle the "done" status of the task
  if (task.done){
    task.completedDate = new Date().toISOString().split('T')[0] //Set current date as completed date
  }
  else{
    task.completedDate = null // Reset completed date if unchecked
  }
  try { 
    await axios.put(`/api/task/${task.id}`, task) 
  }catch (error) { 
    console.error('Error updating task status:', error); 
  } 
};

// Function to start editing a task
const startEditingTask = (task) => {
  editingTask.value = { ...task }
}

// Function to save the updated task
const saveTask = async (task) => {
  try {
    const response = await axios.put(`http://localhost:5000/api/task/${task.id}`, task)
    if (response.status === 204) {
      const index = tasks.value.findIndex(t => t.id === task.id)
      if (index !== -1) {
        tasks.value[index] = task
      }
      editingTask.value = null
    } else {
      console.error('Failed to update task')
    }
  } catch (error) {
    console.error('Error updating task:', error)
  }
}

// Function to cancel editing
const cancelEdit = () => {
  editingTask.value = null
}


</script>

<!-- Add your CSS styling here -->
<style>
.todo-app {
  max-width: 600px;
  margin: 50px auto;
  padding: 20px;
  border: 1px solid #ddd;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  background-color: #f9f9f9;
  text-align: center;
  font-family: Arial, sans-serif;
}

h1 {
  font-size: 24px;
  margin-bottom: 20px;
  color: #333;
}

.input-group {
  display: flex;
  justify-content: space-between;
  margin-bottom: 20px;
  flex-wrap: wrap;
}

.task-input,
.tag-input {
  flex: 1;
  padding: 10px;
  margin-right: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 16px;
  box-sizing: border-box;
  height: 40px;
}

.add-button {
  padding: 10px 12px;
  background-color: #4CAF50;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  margin-top: 10px;
}

.add-button:hover {
  background-color: #45a049;
}

.task-list {
  list-style-type: none;
  padding: 0;
}

.task-item {
  padding: 10px;
  border-bottom: 1px solid #ddd;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.task-tags {
  margin-left: 10px;
}

.tag {
  background-color: #eee;
  color: #333;
  padding: 4px 8px;
  margin-left: 5px;
  border-radius: 12px;
  font-size: 12px;
}

.task-done {
  text-decoration: line-through;
  color: rgb(164, 165, 167);
}

.task-dates {
  font-size: 12px;
  color: #0ef81a;
  margin-top: 5px; 
}

.date-input {
flex: 1;
padding: 10px;
margin-right: 10px;
border: 1px solid #ccc;
border-radius: 4px;
font-size: 16px;
box-sizing: border-box;
height: 40px;
color: #333;
background-color: #eee;
margin-top: 10px;
}

.delete-button {
  background-color: #f44336;
  color: white;
  border: none;
  padding: 5px 10px;
  border-radius: 4px;
  cursor: pointer;
}

.delete-button:hover {
  background-color: #e53935;
}

.edit-button {
  background-color: #2196F3;
  color: white;
  border: none;
  padding: 5px 10px;
  border-radius: 4px;
  cursor: pointer;
}

.edit-button:hover {
  background-color: #1976D2;
}

.cancel-button {
  background-color: #f0ad4e;
  color: white;
  border: none;
  padding: 5px 
}
</style>