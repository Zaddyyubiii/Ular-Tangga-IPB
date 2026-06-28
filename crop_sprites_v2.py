import os
import glob
from PIL import Image

def process_sprites(folder_path):
    files = glob.glob(os.path.join(folder_path, '*.png'))
    if not files:
        print(f"No PNG files found in {folder_path}")
        return

    # Step 1: Find max bounding box dimensions
    max_w = 0
    max_h = 0
    valid_files = []
    
    for f in files:
        img = Image.open(f).convert("RGBA")
        bbox = img.getbbox()
        if bbox:
            w = bbox[2] - bbox[0]
            h = bbox[3] - bbox[1]
            if w > max_w: max_w = w
            if h > max_h: max_h = h
            valid_files.append((f, img, bbox, w, h))
            
    print(f"Max bbox width: {max_w}, Max bbox height: {max_h}")
    
    # Step 2: Define canvas size (square, padded)
    canvas_size = int(max(max_w, max_h) * 1.3)
    # The foot anchor will be at the horizontal center and slightly above the bottom edge
    anchor_x = canvas_size // 2
    anchor_y = int(canvas_size * 0.9) # 10% bottom padding
    
    # Step 3: Process and save all images
    for f, img, bbox, w, h in valid_files:
        # Crop to bbox
        cropped = img.crop(bbox)
        
        # New square canvas
        new_img = Image.new("RGBA", (canvas_size, canvas_size), (0, 0, 0, 0))
        
        # We want the bottom-center of the cropped image to be placed at (anchor_x, anchor_y)
        # bottom-center of cropped is (w//2, h)
        paste_x = anchor_x - (w // 2)
        paste_y = anchor_y - h
        
        new_img.paste(cropped, (paste_x, paste_y))
        new_img.save(f)
        print(f"Properly aligned and cropped: {os.path.basename(f)}")

if __name__ == "__main__":
    sprites_dir = r"c:\Users\LENOVO\Desktop\Ular-Tangga-IPB\web-ui\public\sprites"
    process_sprites(sprites_dir)
    print("All sprites processed successfully with 1:1 consistent centering!")
