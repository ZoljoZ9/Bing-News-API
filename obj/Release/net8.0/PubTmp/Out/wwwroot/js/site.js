// Function to switch theme
function setTheme(theme) {
    // If the theme is 'auto', check the system preference
    if (theme === 'auto') {
        const prefersDarkScheme = window.matchMedia("(prefers-color-scheme: dark)").matches;
        document.documentElement.setAttribute('data-bs-theme', prefersDarkScheme ? 'dark' : 'light');
    } else {
        // Otherwise, apply the selected theme directly (light or dark)
        document.documentElement.setAttribute('data-bs-theme', theme);
    }

    // Save the selected theme in localStorage for persistence
    localStorage.setItem('theme', theme);

    // Update the active icon in the theme picker based on the selected theme
    updateThemeIcon(theme);
}

// Function to update the theme picker icons based on the selected theme
function updateThemeIcon(theme) {
    const themeIcon = document.querySelector('#bd-theme i');
    if (theme === 'light') {
        themeIcon.className = 'bi bi-sun-fill'; // Change icon to sun for light mode
    } else if (theme === 'dark') {
        themeIcon.className = 'bi bi-moon-stars-fill'; // Change icon to moon for dark mode
    } else {
        themeIcon.className = 'bi bi-circle-half'; // Change icon to half-circle for auto mode
    }
}

// Apply the saved theme when the page loads, default to 'auto' if none is saved
document.addEventListener('DOMContentLoaded', function () {
    const savedTheme = localStorage.getItem('theme') || 'auto'; // Default to auto theme if none is saved

    // Set the theme based on the saved preference
    setTheme(savedTheme);

    // If 'auto' is selected, listen for system theme changes and update accordingly
    if (savedTheme === 'auto') {
        window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
            const newColorScheme = e.matches ? 'dark' : 'light';
            document.documentElement.setAttribute('data-bs-theme', newColorScheme);
        });
    }
});

// Attach click events to theme picker buttons
document.getElementById('lightThemeButton').addEventListener('click', function () {
    setTheme('light');
});
document.getElementById('darkThemeButton').addEventListener('click', function () {
    setTheme('dark');
});
document.getElementById('autoThemeButton').addEventListener('click', function () {
    setTheme('auto');
});
