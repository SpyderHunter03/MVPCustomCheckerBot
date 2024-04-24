// Define the type for a single option
export type Option = {
  label: string;
  value: string;
};

// Props for the MultiSelectDropdown component
type MultiSelectDropdownProps = {
  options: Option[];
  value: Option[];
  onChange: (newValue: Option[]) => void;
};

function MultiSelectDropdown({ options, value, onChange }: MultiSelectDropdownProps) {
  const handleOptionClicked = (option: Option) => {
    if (value.includes(option)) {
      onChange(value.filter((v) => v !== option));
    } else {
      onChange([...value, option]);
    }
  };

  return (
    <div className="dropdown">
      <button className="dropbtn">Select Options</button>
      <div className="dropdown-content">
        {options.map((option) => (
          <label key={option.value}>
            <input
              type="checkbox"
              checked={value.some((v) => v.value === option.value)}
              onChange={() => handleOptionClicked(option)}
            />
            {option.label}
          </label>
        ))}
      </div>
    </div>
  );
}

export default MultiSelectDropdown;