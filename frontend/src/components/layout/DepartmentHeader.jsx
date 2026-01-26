import { useState } from 'react';

/**
 * Department Header Component
 * Displays the official header with Haryana Government emblem and department information
 * 
 * Logo: Haryana Government Emblem
 * Place the emblem image in: frontend/public/haryana-emblem.png (or .jpg, .svg)
 */
export default function DepartmentHeader() {
  // Logo path - Haryana Government Emblem
  // Place your emblem image in frontend/public/ directory
  const logoPath = '/haryana-emblem.png'; // Change to '/haryana-emblem.jpg' or '/haryana-emblem.svg' if needed
  const [logoError, setLogoError] = useState(false);

  return (
    <div className="w-full bg-gradient-to-r from-blue-50 to-white border-b-4 border-blue-600 shadow-md">
      <div className="w-full py-4">
        <div className="flex items-center gap-4 pl-4 md:pl-6">
          {/* Logo Section - Haryana Government Emblem */}
          <div className="flex-shrink-0">
            {!logoError ? (
              <img
                src={logoPath}
                alt="Haryana Government Emblem"
                className="h-20 w-20 md:h-24 md:w-24 lg:h-28 lg:w-28 object-contain"
                onError={() => {
                  console.warn('Haryana Government Emblem image not found. Using placeholder.');
                  setLogoError(true);
                }}
              />
            ) : (
              // Placeholder - will be replaced when logo is added
              <div className="h-20 w-20 md:h-24 md:w-24 lg:h-28 lg:w-28 bg-gradient-to-br from-amber-600 to-amber-800 rounded-lg flex items-center justify-center shadow-md">
                <span className="text-white font-bold text-2xl md:text-3xl">H</span>
              </div>
            )}
          </div>

          {/* Text Section - Left Aligned */}
          <div className="flex flex-col justify-center min-w-0">
            {/* Big Font: Department Name - Reduced by 30% total */}
            <h1 className="text-xs md:text-sm lg:text-base xl:text-xl font-bold text-gray-900 leading-tight tracking-tight">
              DEPARTMENT OF ECONOMIC AND STATISTICAL AFFAIRS, HARYANA
            </h1>
            
            {/* Middle Size Font: Project Name - Reduced by 30% total */}
            <h2 className="text-[11px] md:text-xs lg:text-sm xl:text-base font-semibold text-blue-700 mt-1 md:mt-1.5">
              STATISTICAL ABSTRACT OF HARYANA
            </h2>
          </div>
        </div>
      </div>
    </div>
  );
}
