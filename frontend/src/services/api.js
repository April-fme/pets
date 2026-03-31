import axios from 'axios';

const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || process.env.react_app_api_url || 'http://localhost:3001/api',
});

export default api;
