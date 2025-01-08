# Steps to Publish ERAM_2_GEOJSON

## **Make Edits and clean up**
1. Make required edits to code.

2. Search entire solution for "TODO" for reminders.

3. Save.

4. F6 to build.

---

## **Step 2: Publish the Application**
1. Open the **Publish Wizard**:
   - Right-click your project in **Solution Explorer** and select **Publish**.

2. Create or open a publish profile:
   - Select a target folder for publishing.
   - Click **Next**.

3. Configure publishing options:
   - **Deployment Mode**: Select **Self-contained**.
   - **Target Runtime**: `win-x64`.
   - **Produce Single File**: Ensure this is checked.

4. Publish:
   - Click **Finish**, then **Publish**.

---

## **Step 3: Test the Published .exe**
1. Navigate to the publish folder (e.g., `ERAM_2_GEOJSON\bin\Release\net8.0\publish\win-x64`).
2. Confirm the following:
   - Only the `.exe` file is present (the `.pdb` file is optional for debugging).
   - The `.exe` works independently when moved to another folder or system.

3. Test on a machine without the .NET runtime installed to ensure it runs properly.

---

## **Step 4: Distribute the Application**
1. Distribute the `.exe` file.

---

## Notes
- Keep the `.pdb` file only if you need to debug the application. Otherwise, it can be excluded by adding `<DebugType>none</DebugType>` to the `<PropertyGroup>`.
- Always test the application on a clean environment to ensure it works as expected.

