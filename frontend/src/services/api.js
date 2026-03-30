import axios from 'axios';

const api = axios.create({
  baseURL: `https://pets-v48q.onrender.com/api`,
});

export default api;
