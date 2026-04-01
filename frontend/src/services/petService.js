import api from './api';
import authService from './authService';

export const getPets = async () => {
  const makeRequest = async () => {
    // 確保 token 被設置
    const token = authService.getToken();
    if (token) {
      api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    }
    
    const response = await api.get('/pets');
    // .NET API 返回格式為 { value: [...], Count: 2 }
    // 提取 value 陣列或直接返回 data（兼容兩種格式）
    const data = response.data.value || response.data;
    console.log('載入寵物成功:', data);
    return data;
  };

  try {
    return await makeRequest();
  } catch (error) {
    console.warn('第一次加載寵物失敗，2秒後重試...', error.message);
    try {
      await new Promise(resolve => setTimeout(resolve, 2000));
      return await makeRequest();
    } catch (retryError) {
      console.error('第二次加載寵物失敗詳細信息:', {
        status: retryError.response?.status,
        statusText: retryError.response?.statusText,
        message: retryError.message,
        data: retryError.response?.data,
        headers: retryError.response?.request?.headers
      });
      throw retryError;
    }
  }
};
