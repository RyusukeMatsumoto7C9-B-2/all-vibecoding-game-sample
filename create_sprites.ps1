# PowerShell script to create simple sprite textures for Unity
Add-Type -AssemblyName System.Drawing

# Create sprites directory if it doesn't exist
$spritesDir = "Assets\MyGame\Sprites"
if (!(Test-Path $spritesDir)) {
    New-Item -ItemType Directory -Path $spritesDir -Force
}

# Create wall sprite (gray 32x32)
$wallBitmap = New-Object System.Drawing.Bitmap(32, 32)
$graphics = [System.Drawing.Graphics]::FromImage($wallBitmap)
$grayBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 128, 128, 128))
$graphics.FillRectangle($grayBrush, 0, 0, 32, 32)
$wallBitmap.Save("$spritesDir\WallSprite.png", [System.Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose()
$wallBitmap.Dispose()
$grayBrush.Dispose()

# Create ground sprite (brown 32x32)
$groundBitmap = New-Object System.Drawing.Bitmap(32, 32)
$graphics = [System.Drawing.Graphics]::FromImage($groundBitmap)
$brownBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 160, 100, 50))
$graphics.FillRectangle($brownBrush, 0, 0, 32, 32)
$groundBitmap.Save("$spritesDir\GroundSprite.png", [System.Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose()
$groundBitmap.Dispose()
$brownBrush.Dispose()

Write-Host "Sprite textures created successfully!"
Write-Host "- WallSprite.png (gray)"
Write-Host "- GroundSprite.png (brown)"