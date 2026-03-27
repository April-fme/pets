
import './App.css';
import { useEffect, useState } from 'react';
import { getPets } from './services/petService';
import HealthChart from './components/HealthChart';
import LoginPage from './components/LoginPage';
import authService from './services/authService';

function App() {
  const [pets, setPets] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedPet, setSelectedPet] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [currentUser, setCurrentUser] = useState(null);

  useEffect(() => {
    const authenticated = authService.initAuth();
    setIsAuthenticated(authenticated);
    if (authenticated) {
      setCurrentUser(authService.getCurrentUser());
    }
  }, []);

  useEffect(() => {
    if (!isAuthenticated) {
      setLoading(false);
      return;
    }
    setLoading(true);
    getPets()
      .then(data => {
        setPets(data);
        setLoading(false);
        if (data.length > 0) {
          setSelectedPet(data[0]);
        }
      })
      .catch(err => {
        if (err.response?.status === 401) {
          authService.logout();
          setIsAuthenticated(false);
        } else {
          setError('載入寵物資料失敗');
        }
        setLoading(false);
      });
  }, [isAuthenticated]);

  const handleLoginSuccess = () => {
    setCurrentUser(authService.getCurrentUser());
    setIsAuthenticated(true);
  };

  const handleLogout = () => {
    authService.logout();
    setIsAuthenticated(false);
    setCurrentUser(null);
    setPets([]);
    setSelectedPet(null);
    setError(null);
  };

  if (!isAuthenticated) {
    return <LoginPage onLoginSuccess={handleLoginSuccess} />;
  }

  const username = currentUser?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
    || currentUser?.unique_name
    || currentUser?.name
    || '使用者';

  return (
    <div className="App">
      <header className="App-header" style={{ backgroundColor: '#282c34', padding: '20px', color: 'white', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <h1 style={{ margin: 0 }}>🐾 寵物健康管理系統</h1>
        <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
          <span style={{ fontSize: '14px', opacity: 0.8 }}>👤 {username}</span>
          <button
            onClick={handleLogout}
            style={{
              padding: '8px 16px',
              backgroundColor: '#e53935',
              color: 'white',
              border: 'none',
              borderRadius: '6px',
              cursor: 'pointer',
              fontSize: '14px',
            }}
          >
            登出
          </button>
        </div>
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
      </div>
    </div>
  );
}

export default App;
