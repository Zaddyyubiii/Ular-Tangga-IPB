import os
import glob
from PIL import Image

def process_sprites(folder_path):
    files = glob.glob(os.path.join(folder_path, '*.png'))
    if not files:
        print(f"No PNG files found in {folder_path}")
        return

    for file_path in files:
        img = Image.open(file_path).convert("RGBA")
        # Get the bounding box of the non-transparent regions
        bbox = img.getbbox()
        if bbox:
            # bbox is (left, upper, right, lower)
            # Crop to bounding box
            cropped = img.crop(bbox)
            
            # Create a new square image to center it
            # To ensure it fits perfectly in a circle PFP, a square is ideal.
            max_dim = max(cropped.width, cropped.height)
            # Add a little padding (e.g., 10%)
            padded_dim = int(max_dim * 1.2)
            
            new_img = Image.new("RGBA", (padded_dim, padded_dim), (0, 0, 0, 0))
            
            # Center the cropped image in the new square image
            x = (padded_dim - cropped.width) // 2
            y = (padded_dim - cropped.height) // 2
            
            new_img.paste(cropped, (x, y))
            
            # Save it back (overwrite)
            new_img.save(file_path)
            print(f"Processed: {os.path.basename(file_path)}")
        else:
            print(f"Empty image: {os.path.basename(file_path)}")

if __name__ == "__main__":
    sprites_dir = r"c:\Users\LENOVO\Desktop\Ular-Tangga-IPB\web-ui\public\sprites"
    process_sprites(sprites_dir)
    print("Done!")
