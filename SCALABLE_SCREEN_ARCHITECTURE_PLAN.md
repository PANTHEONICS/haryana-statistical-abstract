# Scalable Screen Architecture Plan
## For 350+ Data Entry Screens Organized by Department

---

## 📋 **Overview**

This plan provides a scalable, maintainable architecture for organizing 350+ data entry screens (tables) by department, using the Census screen as the template. All screens will share the same behavior (CRUD, workflow, audit history) with only data structure and columns changing.

---

## 🏗️ **1. Naming Conventions**

### **1.1 Database Table Naming**
**Format:** `{DeptCode}_Table_{TableNumber}_{TableName}`

**Examples:**
- `AP_Table_3_2_CensusPopulation` (Area & Population, Table 3.2)
- `HFW_Table_5_1_HealthIndicators` (Health & Family Welfare, Table 5.1)
- `EDU_Table_7_3_SchoolEnrollment` (Education, Table 7.3)

**Rules:**
- **DeptCode**: 2-3 letter department code (from `Mst_Departments.DepartmentCode`)
- **TableNumber**: Original table number from source document (e.g., "3.2" → "3_2")
- **TableName**: PascalCase descriptive name (e.g., "CensusPopulation")

### **1.2 Screen Code (for Workflow)**
**Format:** `{DeptCode}_TABLE_{TableNumber}_{TableName}`

**Examples:**
- `AP_TABLE_3_2_CENSUS_POPULATION`
- `HFW_TABLE_5_1_HEALTH_INDICATORS`

### **1.3 Frontend File Naming**
**Format:** `Table{TableNumber}_{TableName}.jsx`

**Examples:**
- `Table3_2_CensusPopulation.jsx`
- `Table5_1_HealthIndicators.jsx`

### **1.4 Backend File Naming**
**Format:** `Table{TableNumber}{TableName}{Type}.cs`

**Examples:**
- `Table3_2CensusPopulationController.cs`
- `Table3_2CensusPopulationService.cs`
- `Table3_2CensusPopulation.cs` (Model)
- `Table3_2CensusPopulationDto.cs` (DTO)

### **1.5 API Endpoint Naming**
**Format:** `/api/v1/{DeptCode}/Table{TableNumber}/{TableName}`

**Examples:**
- `/api/v1/AP/Table3_2/CensusPopulation`
- `/api/v1/HFW/Table5_1/HealthIndicators`

---

## 📁 **2. Folder Structure**

### **2.1 Frontend Structure**
```
frontend/src/
├── pages/
│   ├── AreaAndPopulation/
│   │   ├── Table3_2_CensusPopulation.jsx
│   │   ├── Table3_3_PopulationDensity.jsx
│   │   └── index.js (barrel export)
│   ├── HealthAndFamilyWelfare/
│   │   ├── Table5_1_HealthIndicators.jsx
│   │   ├── Table5_2_HospitalBeds.jsx
│   │   └── index.js
│   ├── Education/
│   │   ├── Table7_1_Schools.jsx
│   │   └── index.js
│   └── ... (other departments)
│
├── services/
│   ├── AreaAndPopulation/
│   │   ├── table3_2_censusPopulationApi.js
│   │   └── index.js
│   ├── HealthAndFamilyWelfare/
│   │   ├── table5_1_healthIndicatorsApi.js
│   │   └── index.js
│   └── ... (other departments)
│
└── components/
    └── screens/ (shared screen components)
        ├── ScreenTemplate.jsx (base template)
        └── ScreenConfig.js (configuration helper)
```

### **2.2 Backend Structure**
```
backend/HaryanaStatAbstract.API/
├── Controllers/
│   ├── AreaAndPopulation/
│   │   ├── Table3_2CensusPopulationController.cs
│   │   └── ... (other controllers)
│   ├── HealthAndFamilyWelfare/
│   │   ├── Table5_1HealthIndicatorsController.cs
│   │   └── ... (other controllers)
│   └── ... (other departments)
│
├── Services/
│   ├── AreaAndPopulation/
│   │   ├── Table3_2CensusPopulationService.cs
│   │   ├── ITable3_2CensusPopulationService.cs
│   │   └── ... (other services)
│   ├── HealthAndFamilyWelfare/
│   │   └── ... (other services)
│   └── ... (other departments)
│
├── Models/
│   ├── AreaAndPopulation/
│   │   ├── Table3_2CensusPopulation.cs
│   │   ├── Dtos/
│   │   │   ├── Table3_2CensusPopulationDto.cs
│   │   │   ├── CreateTable3_2CensusPopulationDto.cs
│   │   │   └── UpdateTable3_2CensusPopulationDto.cs
│   │   └── ... (other models)
│   ├── HealthAndFamilyWelfare/
│   │   └── ... (other models)
│   └── ... (other departments)
│
└── Data/
    └── ApplicationDbContext.cs (registers all DbSets)
```

### **2.3 Database Scripts Structure**
```
backend/database/
├── departments/
│   ├── AreaAndPopulation/
│   │   ├── AP_Table_3_2_CensusPopulation.sql
│   │   └── AP_Table_3_3_PopulationDensity.sql
│   ├── HealthAndFamilyWelfare/
│   │   ├── HFW_Table_5_1_HealthIndicators.sql
│   │   └── ... (other tables)
│   └── ... (other departments)
│
└── master/
    ├── Mst_ScreenRegistry.sql (NEW - tracks all screens)
    └── ... (existing master tables)
```

---

## 🗄️ **3. Master Screen Registry Table**

### **3.1 Purpose**
A master table to track all 350+ screens, their metadata, and relationships.

### **3.2 Schema: `Mst_ScreenRegistry`**
```sql
CREATE TABLE [dbo].[Mst_ScreenRegistry] (
    [ScreenRegistryID] INT IDENTITY(1,1) PRIMARY KEY,
    [ScreenCode] NVARCHAR(100) NOT NULL UNIQUE, -- e.g., "AP_TABLE_3_2_CENSUS_POPULATION"
    [ScreenName] NVARCHAR(200) NOT NULL, -- e.g., "Census Population Management"
    [TableName] NVARCHAR(100) NOT NULL UNIQUE, -- e.g., "AP_Table_3_2_CensusPopulation"
    [DepartmentID] INT NOT NULL, -- FK to Mst_Departments
    [TableNumber] NVARCHAR(50) NOT NULL, -- e.g., "3.2"
    [SourceDocument] NVARCHAR(200) NULL, -- e.g., "Statistical Abstract of Haryana 2023-24"
    [SourceTable] NVARCHAR(100) NULL, -- e.g., "Table 3.2"
    [Description] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [DisplayOrder] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    [CreatedByUserID] INT NULL,
    
    CONSTRAINT [FK_Mst_ScreenRegistry_Department] 
        FOREIGN KEY ([DepartmentID]) REFERENCES [dbo].[Mst_Departments]([DepartmentID]),
    CONSTRAINT [FK_Mst_ScreenRegistry_CreatedBy] 
        FOREIGN KEY ([CreatedByUserID]) REFERENCES [dbo].[Master_User]([UserID])
);

CREATE INDEX [IX_Mst_ScreenRegistry_DepartmentID] ON [dbo].[Mst_ScreenRegistry]([DepartmentID]);
CREATE INDEX [IX_Mst_ScreenRegistry_ScreenCode] ON [dbo].[Mst_ScreenRegistry]([ScreenCode]);
CREATE INDEX [IX_Mst_ScreenRegistry_TableName] ON [dbo].[Mst_ScreenRegistry]([TableName]);
```

### **3.3 Benefits**
- **Centralized tracking**: All screens in one place
- **Dynamic menu generation**: Generate menus from registry
- **Screen discovery**: Easy to find screens by department/table number
- **Metadata management**: Store source document, description, etc.

---

## 🔄 **4. Migration Strategy**

### **4.1 Step 1: Create Master Screen Registry**
1. Create `Mst_ScreenRegistry` table
2. Migrate existing Census screen entry
3. Create C# model and service for registry

### **4.2 Step 2: Reorganize Census Screen**
1. **Database:**
   - Rename `census_population` → `AP_Table_3_2_CensusPopulation`
   - Update `Screen_Workflow.ScreenCode` → `AP_TABLE_3_2_CENSUS_POPULATION`

2. **Backend:**
   - Move `CensusPopulationController.cs` → `Controllers/AreaAndPopulation/Table3_2CensusPopulationController.cs`
   - Move `CensusPopulationService.cs` → `Services/AreaAndPopulation/Table3_2CensusPopulationService.cs`
   - Move `CensusPopulation.cs` → `Models/AreaAndPopulation/Table3_2CensusPopulation.cs`
   - Update namespaces and class names
   - Update API route to `/api/v1/AP/Table3_2/CensusPopulation`

3. **Frontend:**
   - Move `Census.jsx` → `pages/AreaAndPopulation/Table3_2_CensusPopulation.jsx`
   - Move `censusApi.js` → `services/AreaAndPopulation/table3_2_censusPopulationApi.js`
   - Update imports and component names
   - Update route path

### **4.3 Step 3: Create Screen Generator Tool**
A CLI tool or script to generate new screens from the Census template.

---

## 🛠️ **5. Screen Generator Tool**

### **5.1 Purpose**
Generate new screens by copying the Census template and replacing:
- Table names
- Column definitions
- Screen codes
- File paths
- API endpoints
- Component names

### **5.2 Input Parameters**
```json
{
  "departmentCode": "AP",
  "departmentName": "AreaAndPopulation",
  "tableNumber": "3.3",
  "tableName": "PopulationDensity",
  "screenName": "Population Density Management",
  "columns": [
    { "name": "Year", "type": "int", "required": true },
    { "name": "Density", "type": "decimal(10,2)", "required": true }
  ]
}
```

### **5.3 Generated Files**
1. **Database:** `AP_Table_3_3_PopulationDensity.sql`
2. **Backend Model:** `Models/AreaAndPopulation/Table3_3PopulationDensity.cs`
3. **Backend DTOs:** `Models/AreaAndPopulation/Dtos/Table3_3PopulationDensityDto.cs`
4. **Backend Service:** `Services/AreaAndPopulation/Table3_3PopulationDensityService.cs`
5. **Backend Controller:** `Controllers/AreaAndPopulation/Table3_3PopulationDensityController.cs`
6. **Frontend API:** `services/AreaAndPopulation/table3_3_populationDensityApi.js`
7. **Frontend Page:** `pages/AreaAndPopulation/Table3_3_PopulationDensity.jsx`
8. **Screen Registry Entry:** Insert into `Mst_ScreenRegistry`
9. **Screen Workflow Entry:** Insert into `Screen_Workflow`

---

## 📝 **6. Implementation Checklist**

### **Phase 1: Foundation (Week 1)**
- [ ] Create `Mst_ScreenRegistry` table and model
- [ ] Create `ScreenRegistryService` for managing screens
- [ ] Create folder structure for departments
- [ ] Create screen generator tool (CLI or script)

### **Phase 2: Migrate Census Screen (Week 1-2)**
- [ ] Rename database table `census_population` → `AP_Table_3_2_CensusPopulation`
- [ ] Move backend files to `AreaAndPopulation/` folder
- [ ] Update namespaces, class names, API routes
- [ ] Move frontend files to `AreaAndPopulation/` folder
- [ ] Update imports, component names, routes
- [ ] Test Census screen after migration
- [ ] Register Census screen in `Mst_ScreenRegistry`

### **Phase 3: Generate Next 10 Screens (Week 2-3)**
- [ ] Use generator tool to create 10 screens
- [ ] Test each screen
- [ ] Refine generator tool based on issues

### **Phase 4: Scale to 350 Screens (Week 3-8)**
- [ ] Batch generate remaining screens
- [ ] Quality assurance testing
- [ ] Performance optimization
- [ ] Documentation

---

## 🎯 **7. Benefits of This Architecture**

### **7.1 Organization**
- ✅ Clear department-based structure
- ✅ Easy to locate screens by department or table number
- ✅ Scalable to 1000+ screens

### **7.2 Maintainability**
- ✅ Consistent naming conventions
- ✅ Predictable file locations
- ✅ Easy to refactor or move screens

### **7.3 Developer Experience**
- ✅ Quick screen generation from template
- ✅ Reduced code duplication
- ✅ Clear patterns to follow

### **7.4 Database Management**
- ✅ Easy to identify tables by prefix
- ✅ Department-based organization
- ✅ Centralized screen registry

---

## 📊 **8. Example: Complete Screen Structure**

### **8.1 Database Table**
```sql
CREATE TABLE [dbo].[AP_Table_3_2_CensusPopulation] (
    [CensusID] INT IDENTITY(1,1) PRIMARY KEY,
    [census_year] INT NOT NULL UNIQUE,
    [total_population] BIGINT NOT NULL,
    -- ... other columns
);
```

### **8.2 Backend Model**
```csharp
namespace HaryanaStatAbstract.API.Models.AreaAndPopulation
{
    [Table("AP_Table_3_2_CensusPopulation")]
    public class Table3_2CensusPopulation
    {
        [Key]
        [Column("CensusID")]
        public int CensusID { get; set; }
        // ... other properties
    }
}
```

### **8.3 Backend Controller**
```csharp
namespace HaryanaStatAbstract.API.Controllers.AreaAndPopulation
{
    [ApiController]
    [Route("api/v1/AP/Table3_2/CensusPopulation")]
    public class Table3_2CensusPopulationController : ControllerBase
    {
        // ... implementation
    }
}
```

### **8.4 Frontend Page**
```javascript
// frontend/src/pages/AreaAndPopulation/Table3_2_CensusPopulation.jsx
import table3_2_censusPopulationApi from "@/services/AreaAndPopulation/table3_2_censusPopulationApi";

export default function Table3_2_CensusPopulation() {
    // ... implementation (same as Census.jsx)
}
```

### **8.5 Frontend API Service**
```javascript
// frontend/src/services/AreaAndPopulation/table3_2_censusPopulationApi.js
import BaseApiService from '../BaseApiService';

class Table3_2CensusPopulationApiService extends BaseApiService {
    constructor() {
        super('/AP/Table3_2/CensusPopulation');
    }
}

export default new Table3_2CensusPopulationApiService();
```

---

## 🚀 **9. Next Steps**

1. **Review and approve this plan**
2. **Create `Mst_ScreenRegistry` table**
3. **Set up folder structure**
4. **Migrate Census screen as proof of concept**
5. **Build screen generator tool**
6. **Generate remaining screens**

---

## 📚 **10. Documentation**

- Each department folder should have a `README.md` describing its screens
- Screen generator tool should have usage documentation
- `Mst_ScreenRegistry` should be documented in database schema docs

---

**This architecture ensures scalability, maintainability, and ease of development for 350+ screens!** 🎉
