// utils/axios.js
import axios from 'axios';

const instance = axios.create({
  baseURL: 'http://localhost:5000', // Base URL of your API
  timeout: 1000,
  headers: { 'Content-Type': 'application/json' },
});

export default instance;