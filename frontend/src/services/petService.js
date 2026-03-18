import api from './api';

export const getPets = async () => {
  const response = await api.get('/pets');
  // .NET API 返回格式為 { value: [...], Count: 2 }
  // 提取 value 陣列或直接返回 data（兼容兩種格式）
  return response.data.value || response.data;
};
