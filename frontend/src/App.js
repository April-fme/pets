
import './App.css';
import { useEffect, useState } from 'react';
import { getPets } from './services/petService';
import HealthChart from './components/HealthChart';

function App() {
  const [pets, setPets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedPet, setSelectedPet] = useState(null);

  useEffect(() => {
    getPets()
      .then(data => {
        setPets(data);
        setLoading(false);
        // 預設選擇第一隻寵物
        if (data.length > 0) {
          setSelectedPet(data[0]);
        }
      })
      .catch(err => {
        setError('載入寵物資料失敗');
        setLoading(false);
      });
  }, []);

  return (
    <div className="App">
      <header className="App-header" style={{ backgroundColor: '#282c34', padding: '20px', color: 'white' }}>
        <h1>🐾 寵物健康管理系統</h1>
      </header>
      
      <div style={{ padding: '20px' }}>
        <section style={{ marginBottom: '30px' }}>
          <h2>寵物列表</h2>
          {loading && <p>載入中...</p>}
          {error && <p style={{color: 'red'}}>{error}</p>}
          <div style={{ display: 'flex', gap: '10px', flexWrap: 'wrap' }}>
            {pets.map((pet) => (
              <button 
                key={pet.id}
                onClick={() => setSelectedPet(pet)}
                style={{
                  padding: '10px 20px',
                  backgroundColor: selectedPet?.id === pet.id ? '#4CAF50' : '#ddd',
                  color: selectedPet?.id === pet.id ? 'white' : 'black',
                  border: 'none',
                  borderRadius: '5px',
                  cursor: 'pointer',
                  fontSize: '16px'
                }}
              >
                {pet.name} ({pet.species})
              </button>
            ))}
          </div>
        </section>

        {selectedPet && (
          <section>
            <h2>📊 健康指標監控</h2>
            <HealthChart
              petId={selectedPet.id}
              petName={selectedPet.name}
            />
          </section>
        )}

        {/* 舊版圖表（已停用） */}
        {/* <ActivityChartAPI petId={selectedPet?.id} petName={selectedPet?.name} /> */}
      </div>
    </div>
  );
}

export default App;
