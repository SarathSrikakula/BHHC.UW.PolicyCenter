import React, { useState, useMemo } from 'react';
import TextField from '@mui/material/TextField';
import Autocomplete from '@mui/material/Autocomplete';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';

// --- Data Simulation ---
// Updated to use objects with 'id' and 'text' properties
const allAvailableOptions = [
  { id: '1', text: 'Apple' },
  { id: '2', text: 'Banana' },
  { id: '3', text: 'Cherry' },
  { id: '4', text: 'Date' },
  { id: '5', text: 'Elderberry' },
  { id: '6', text: 'Fig' },
  { id: '7', text: 'Grape' },
  { id: '8', text: 'Honeydew' },
  { id: '9', text: 'Kiwi' },
  { id: '10', text: 'Lemon' },
  { id: '11', text: 'Mango' }, // Default item
  { id: '12', text: 'Nectarine' },
  { id: '13', text: 'Orange' },
  { id: '14', text: 'Papaya' },
  { id: '15', text: 'Quince' },
  { id: '16', text: 'Raspberry' },
  { id: '17', text: 'Strawberry' },
  { id: '18', text: 'Tangerine' },
  { id: '19', text: 'Ugli Fruit' },
  { id: '20', text: 'Vanilla Bean' },
  { id: '21', text: 'Watermelon' },
  { id: '22', text: 'Xigua' },
  { id: '23', text: 'Yellow Plum' },
  { id: '24', text: 'Zucchini' }
];

/**
 * Simulates an asynchronous data fetch with a delay.
 * Filters options based on the 'text' property.
 * @param {string} query - The search query to filter options by.
 * @returns {Promise<Array<{id: string, text: string}>>} A promise that resolves with filtered options.
 */
const fetchFilteredOptions = (query) => {
    return new Promise((resolve) => {
        setTimeout(() => {
            const filtered = allAvailableOptions.filter(option =>
              option.text.toLowerCase().includes(query.toLowerCase())
            );
            resolve(filtered);
        }, 300); // Simulate a network delay of 300ms
    });
};

function App()
{
    // Find the 'Mango' object to set as default
    const defaultMango = allAvailableOptions.find(option => option.text === 'Mango');

    // State for the currently selected value (when an option is chosen)
    // Set default to the 'Mango' object
    const [selectedValue, setSelectedValue] = useState(defaultMango);
    // State for the text currently in the input field
    // Set initial inputValue to the text of the default selected value
    const [inputValue, setInputValue] = useState(defaultMango ? defaultMango.text : '');
    // State for the options displayed in the dropdown
    // Initially, if a default value is set, ensure it's in the displayed options.
    // Otherwise, start empty.
    const [displayedOptions, setDisplayedOptions] = useState(defaultMango ? [defaultMango] : []);
    // State for showing a loading indicator
    const [loading, setLoading] = useState(false);

    // Use useMemo to debounce the API call based on inputValue changes
    useMemo(() => {
        const fetchDebouncedOptions = async () => {
            // If inputValue is empty AND there's no selected value, clear options.
            if (inputValue === '' && !selectedValue)
            {
                setDisplayedOptions([]);
                return;
            }

            // If there's a selected value and the input matches it, ensure it's in options.
            // This explicitly handles the initial load and when a known selection is made.
            if (selectedValue && inputValue === selectedValue.text)
            {
                setDisplayedOptions([selectedValue]); // Display only the selected item as an option
                setLoading(false); // No loading needed as it's already known
                return;
            }

            setLoading(true);
            try
            {
                const result = await fetchFilteredOptions(inputValue);
                // Ensure that if a value is selected (and not cleared by typing),
                // it remains in the options even if filtering temporarily excludes it.
                const finalOptions = selectedValue && !result.some(opt => opt.id === selectedValue.id)
                  ? [selectedValue, ...result]
                  : result;
                setDisplayedOptions(finalOptions);
            }
            catch (error)
            {
                console.error("Error fetching options:", error);
                setDisplayedOptions([]); // Clear options on error
            }
            finally
            {
                setLoading(false);
            }
        };

        const handler = setTimeout(() => {
            fetchDebouncedOptions();
        }, 500); // Debounce time: wait 500ms after last keystroke

        return () => {
            clearTimeout(handler); // Clear timeout if input changes before the delay
        };
    }, [inputValue, selectedValue]); // Added selectedValue to dependencies to react to selection changes

    return (
  
      < Box
      sx ={
        {
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        minHeight: '100vh',
        backgroundColor: '#f5f5f5',
        p: 3,
        fontFamily: 'Inter, sans-serif',
      }
    }
    >
      < Box
        sx ={
        {
        backgroundColor: '#ffffff',
          p: 4,
          borderRadius: '12px',
          boxShadow: '0px 8px 20px rgba(0, 0, 0, 0.1)',
          width: '100%',
          maxWidth: '500px',
          display: 'flex',
          flexDirection: 'column',
          gap: 3,
          alignItems: 'center',
        }
    }
      >
        < Typography variant = "h5" component = "h1" gutterBottom sx = { { color: '#333', fontWeight: 'bold' }}>
          Dynamic Fruit Search 🍎
        </Typography>

        <Autocomplete
          // Controls the selected value of the Autocomplete. It's now an object.
          value={selectedValue}
          // Callback when an option is selected or cleared
          onChange ={
    (event, newValue) => {
        setSelectedValue(newValue);
        // Update inputValue to match the text of the selected object
        setInputValue(newValue ? newValue.text : '');
    }
}
// Controls the text displayed in the input field
inputValue ={ inputValue}
// Callback when the input value changes (e.g., user types)
onInputChange ={
    (event, newInputValue) => {
        setInputValue(newInputValue);
        // Clear selected value if input text doesn't match the current selected item's text
        // This handles cases where user types over a selected value
        if (selectedValue && newInputValue !== selectedValue.text)
        {
            setSelectedValue(null);
        }
        else if (!selectedValue && newInputValue === '')
        {
            // If there's no selected value and input becomes empty, ensure displayedOptions are cleared
            setDisplayedOptions([]);
        }
    }
}
// Options to display in the dropdown. These are now objects.
options ={ displayedOptions}
// Tells Autocomplete how to get the label string from an option object
getOptionLabel ={ (option) => option.text || ""}
// Crucial for comparing options and values (e.g., when setting default or checking selection)
// Compares options based on their 'id' property
isOptionEqualToValue ={ (option, value) => option.id === value.id}
// Whether to allow arbitrary input or only values from options
freeSolo
// Displays a loading indicator while options are being fetched
loading = { loading }
          // Render the input field
          renderInput={(params) => (
            <TextField
              {...params}
              label = "Type to search for a fruit..."
              variant = "outlined"
              fullWidth
              InputProps ={{
                ...params.InputProps,
                endAdornment: (
                  <>
                    { loading ? < CircularProgress color = "inherit" size ={ 20} /> : null}
{params.InputProps.endAdornment}
                  </>
                ),
              }}
              sx ={ { '& .MuiOutlinedInput-root': { borderRadius: '8px' } } }
            />
          )}
          // Custom render for each option in the dropdown (optional)
          renderOption ={
    (props, option) => (
            < li { ...props}>
              < Typography variant = "body1" >
                { option.text}
              </ Typography >
            </ li >
          )}
sx ={ { width: '100%', maxWidth: '400px' } }
        />

        < Box sx ={ { mt: 2, p: 2, backgroundColor: '#e0f7fa', borderRadius: '8px', width: '100%' } }>
          < Typography variant = "body1" sx ={ { color: '#00796b', fontWeight: 'medium' } }>
            { selectedValue ? `Selected: ${ selectedValue.text} (ID: ${ selectedValue.id})` : 'No fruit selected yet.'}
          </ Typography >
          < Typography variant = "body2" sx ={ { color: '#424242', mt: 1 } }>
            Input Text: "{inputValue}"
          </ Typography >
        </ Box >
      </ Box >
    </ Box >
  );
}

export default App;
