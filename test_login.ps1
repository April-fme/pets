$body = @{
    username = "admin"
    password = "admin123"
} | ConvertTo-Json

$response = Invoke-WebRequest -Uri 'http://localhost:5001/api/auth/login' -Method POST -Body $body -ContentType 'application/json' -ErrorAction Stop
$result = $response | ConvertFrom-Json

Write-Host "Status: $($response.StatusCode)"
Write-Host "Message: $($result.message)"
Write-Host "Token: $($result.token.Substring(0, 30))..."
