import axios from 'axios';

let baseURL = 'http://localhost:3001/api';

// 嘗試從 config.json 讀取（生產環境）
const loadConfig = async () => {
  try {
    const response = await fetch('/config.json');
    if (response.ok) {
      const config = await response.json();
      baseURL = config.API_URL || baseURL;
    }
  } catch (error) {
    console.warn('無法讀取 config.json，使用默認 API URL');
  }
};

loadConfig();

const api = axios.create({
  baseURL,
});

export default api;
