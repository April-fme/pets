import api from './api';
import authService from './authService';

export const getPets = async () => {
  try {
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
  } catch (error) {
    console.error('載入寵物失敗詳細信息:', {
      status: error.response?.status,
      statusText: error.response?.statusText,
      message: error.message,
      data: error.response?.data,
      headers: error.response?.request?.headers
    });
    throw error;
  }
};
