/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        game: {
          dark: "#0d0f1a",
          blue: "#0062ff",
          cyan: "#00ccff",
          green: "#15b85a",
          red: "#e63e27",
          yellow: "#f2c029",
          orange: "#f35a27",
          purple: "#9b51e0",
          cardBg: "rgba(13, 17, 35, 0.82)",
          glassBg: "rgba(255, 255, 255, 0.08)",
        }
      },
      boxShadow: {
        bubble: "0 6px 0 0 rgba(0, 0, 0, 0.15)",
        bubbleActive: "0 2px 0 0 rgba(0, 0, 0, 0.15)",
        glowBlue: "0 0 15px rgba(0, 98, 255, 0.4)",
        glowCyan: "0 0 15px rgba(0, 204, 255, 0.4)",
        glowGreen: "0 0 15px rgba(21, 184, 90, 0.4)",
        glowRed: "0 0 15px rgba(230, 62, 39, 0.4)",
        glowYellow: "0 0 15px rgba(242, 192, 41, 0.4)",
      },
      borderRadius: {
        cartoon: "20px",
      },
      fontFamily: {
        playful: ["'Outfit'", "'Inter'", "sans-serif"],
      }
    },
  },
  plugins: [],
}
