function Create-Sprite {
    param(
        [string]$ColorString,
        [string]$OutputPath = "Assets/MyGame/Sprites"
    )
    
    # Parse arguments
    if ($ColorString -match '\((\d+(?:\.\d+)?),\s*(\d+(?:\.\d+)?),\s*(\d+(?:\.\d+)?),\s*(\d+(?:\.\d+)?)\)') {
        $r = [float]$Matches[1]
        $g = [float]$Matches[2]
        $b = [float]$Matches[3]
        $a = [float]$Matches[4]
        
        # Convert from 0-1 range to 0-255 range (if value is 1 or less)
        if ($r -le 1.0) { $r = [int]($r * 255) } else { $r = [int]$r }
        if ($g -le 1.0) { $g = [int]($g * 255) } else { $g = [int]$g }
        if ($b -le 1.0) { $b = [int]($b * 255) } else { $b = [int]$b }
        if ($a -le 1.0) { $a = [int]($a * 255) } else { $a = [int]$a }
        
        # Range check
        $r = [Math]::Max(0, [Math]::Min(255, $r))
        $g = [Math]::Max(0, [Math]::Min(255, $g))
        $b = [Math]::Max(0, [Math]::Min(255, $b))
        $a = [Math]::Max(0, [Math]::Min(255, $a))
    } else {
        Write-Host "Error: Invalid color format. Please use (R,G,B,A) format."
        Write-Host "Example: (255,255,255,255) or (1.0,1.0,1.0,1.0)"
        return
    }
    
    # Create output directory
    $fullOutputPath = Join-Path $PWD $OutputPath
    if (-not (Test-Path $fullOutputPath)) {
        New-Item -ItemType Directory -Path $fullOutputPath -Force
    }
    
    # Generate filename
    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $fileName = "sprite_${r}_${g}_${b}_${a}_${timestamp}.png"
    $filePath = Join-Path $fullOutputPath $fileName
    
    # Create PNG image using .NET System.Drawing
    Add-Type -AssemblyName System.Drawing
    
    $bitmap = New-Object System.Drawing.Bitmap(64, 64)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    
    # Fill with specified color
    $color = [System.Drawing.Color]::FromArgb($a, $r, $g, $b)
    $brush = New-Object System.Drawing.SolidBrush($color)
    $graphics.FillRectangle($brush, 0, 0, 64, 64)
    
    # Save as PNG format
    $bitmap.Save($filePath, [System.Drawing.Imaging.ImageFormat]::Png)
    
    # Release resources
    $graphics.Dispose()
    $bitmap.Dispose()
    $brush.Dispose()
    
    Write-Host "Sprite created: $filePath"
    Write-Host "Size: 64x64"
    Write-Host "Color: ARGB($a, $r, $g, $b)"
}

Export-ModuleMember -Function Create-Sprite