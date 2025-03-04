using Microsoft.Playwright;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SchoolManagement.Playwright.PageObjects
{
    public class ClassesPage
    {
        private readonly IPage _page;
        private const string PageName = "Classes";

        public ClassesPage(IPage page)
        {
            _page = page;
        }

        public async Task NavigateToClassesPage()
        {
            System.Console.WriteLine("  [ClassesPage] Starting navigation to Classes page");
            var sw = Stopwatch.StartNew();
            
            // Navigate to Classes page
            var classesLink = _page.Locator("nav a.nav-link", new() { HasText = PageName });
            System.Console.WriteLine($"  [ClassesPage] Found nav link in {sw.ElapsedMilliseconds}ms");
            
            await classesLink.ClickAsync(new LocatorClickOptions { Force = true });
            System.Console.WriteLine($"  [ClassesPage] Clicked nav link in {sw.ElapsedMilliseconds}ms");
            
            var networkSw = Stopwatch.StartNew();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            networkSw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Waited for network idle in {networkSw.ElapsedMilliseconds}ms");
            
            sw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Navigation completed in {sw.ElapsedMilliseconds}ms");
        }

        public async Task ClickAddNewClassButton()
        {
            System.Console.WriteLine("  [ClassesPage] Clicking Add New Class button");
            var sw = Stopwatch.StartNew();
            
            var addButton = _page.GetByRole(AriaRole.Button, new() { Name = "Add New Class" });
            System.Console.WriteLine($"  [ClassesPage] Found Add New Class button in {sw.ElapsedMilliseconds}ms");
            
            await addButton.ClickAsync(new LocatorClickOptions { Force = true });
            System.Console.WriteLine($"  [ClassesPage] Clicked Add New Class button in {sw.ElapsedMilliseconds}ms");
            
            await _page.WaitForSelectorAsync("div.modal-header:has-text('Add Class')");
            System.Console.WriteLine($"  [ClassesPage] Modal appeared in {sw.ElapsedMilliseconds}ms");
            
            sw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Add button click completed in {sw.ElapsedMilliseconds}ms");
        }

        public async Task FillClassForm(string name, string classCode, string description)
        {
            System.Console.WriteLine("  [ClassesPage] Filling class form");
            var sw = Stopwatch.StartNew();
            
            var addClassModal = _page.Locator("div.modal-content:has(div.modal-header:has-text('Add Class'))");
            System.Console.WriteLine($"  [ClassesPage] Found modal in {sw.ElapsedMilliseconds}ms");
            
            // Fill each field and log time
            await addClassModal.Locator("input#name").FillAsync(name);
            System.Console.WriteLine($"  [ClassesPage] Filled name field in {sw.ElapsedMilliseconds}ms");
            
            await addClassModal.Locator("input#classCode").FillAsync(classCode);
            System.Console.WriteLine($"  [ClassesPage] Filled classCode field in {sw.ElapsedMilliseconds}ms");
            
            await addClassModal.Locator("textarea#description").FillAsync(description);
            System.Console.WriteLine($"  [ClassesPage] Filled description field in {sw.ElapsedMilliseconds}ms");
            
            sw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Form filling completed in {sw.ElapsedMilliseconds}ms");
        }

        public async Task SubmitClassForm()
        {
            System.Console.WriteLine("  [ClassesPage] Submitting class form");
            var sw = Stopwatch.StartNew();
            
            var addClassModal = _page.Locator("div.modal-content:has(div.modal-header:has-text('Add Class'))");
            var addButton = addClassModal.Locator("button:has-text('Add Class')");
            System.Console.WriteLine($"  [ClassesPage] Found Add Class button in {sw.ElapsedMilliseconds}ms");
            
            await addButton.ClickAsync(new LocatorClickOptions { Force = true });
            System.Console.WriteLine($"  [ClassesPage] Clicked Add Class button in {sw.ElapsedMilliseconds}ms");
            
            try {
                var modalCloseSw = Stopwatch.StartNew();
                await _page.WaitForSelectorAsync("#addClassModal:not(.show)", new PageWaitForSelectorOptions { 
                    State = WaitForSelectorState.Visible, 
                    Timeout = 1000 
                });
                modalCloseSw.Stop();
                System.Console.WriteLine($"  [ClassesPage] Modal closed in {modalCloseSw.ElapsedMilliseconds}ms");
            } catch (Exception ex) {
                System.Console.WriteLine($"  [ClassesPage] Modal did not close: {ex.Message}");
                System.Console.WriteLine($"  [ClassesPage] Attempting to force close modal after {sw.ElapsedMilliseconds}ms");
                
                await _page.EvaluateAsync("document.querySelector('#addClassModal.show .btn-close')?.click()");
                System.Console.WriteLine($"  [ClassesPage] Clicked close button via JS in {sw.ElapsedMilliseconds}ms");
                
                await Task.Delay(1000);
                System.Console.WriteLine($"  [ClassesPage] Waited 1000ms after JS click");
                
                await _page.Keyboard.PressAsync("Escape");
                System.Console.WriteLine($"  [ClassesPage] Pressed Escape key in {sw.ElapsedMilliseconds}ms");
                
                await Task.Delay(1000);
                System.Console.WriteLine($"  [ClassesPage] Waited 1000ms after Escape key");
            }
            
            var networkSw = Stopwatch.StartNew();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            networkSw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Waited for network idle in {networkSw.ElapsedMilliseconds}ms");
            
            sw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Form submission completed in {sw.ElapsedMilliseconds}ms");
        }

        public async Task<bool> VerifyClassAdded(string name, string classCode)
        {
            System.Console.WriteLine("  [ClassesPage] Verifying class was added");
            var sw = Stopwatch.StartNew();
            
            var tableSw = Stopwatch.StartNew();
            await _page.WaitForSelectorAsync("table", new PageWaitForSelectorOptions { Timeout = 60000 });
            tableSw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Table appeared in {tableSw.ElapsedMilliseconds}ms");
            
            var classRows = _page.Locator("tr", new() { HasText = name }).Filter(new() { HasText = classCode });
            System.Console.WriteLine($"  [ClassesPage] Located class rows in {sw.ElapsedMilliseconds}ms");
            
            var classRowCount = await classRows.CountAsync();
            System.Console.WriteLine($"  [ClassesPage] Counted {classRowCount} matching rows in {sw.ElapsedMilliseconds}ms");
            
            sw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Verification completed in {sw.ElapsedMilliseconds}ms");
            
            return classRowCount > 0;
        }

        public async Task AddClass(string name, string classCode, string description)
        {
            await NavigateToClassesPage();
            await ClickAddNewClassButton();
            await FillClassForm(name, classCode, description);
            await SubmitClassForm();
            await VerifyClassAdded(name, classCode);
        }

    public async Task SelectTeacherFromDropdown(int index)
    {
        await Task.Delay(500);
        System.Console.WriteLine("  [ClassesPage] Selecting teacher from dropdown");
        var sw = Stopwatch.StartNew();
        
        // Use specific ID selector for teacher dropdown instead of generic "select"
        var teacherDropdown = _page.Locator("select#teacherId");
        System.Console.WriteLine($"  [ClassesPage] Found teacher dropdown in {sw.ElapsedMilliseconds}ms");
        
        // "select#teacherId" に対して2個目のオプションを選択する
        await _page.SelectOptionAsync("select#teacherId", new SelectOptionValue { Index = index });
        
        // // Get all options and find one that contains the teacher name (not exact match)
        // var options = await teacherDropdown.Locator("option").AllAsync();
        // System.Console.WriteLine($"  [ClassesPage] Found {options.Count} options in {sw.ElapsedMilliseconds}ms");
        //
        // string optionValue = null;
        // foreach (var option in options)
        // {
        //     var text = await option.TextContentAsync();
        //     if (text != null && text.Contains(teacherName, StringComparison.OrdinalIgnoreCase))
        //     {
        //         optionValue = await option.GetAttributeAsync("value");
        //         System.Console.WriteLine($"  [ClassesPage] Found matching option with value: {optionValue} in {sw.ElapsedMilliseconds}ms");
        //         break;
        //     }
        // }
        //
        // if (optionValue == null)
        // {
        //     throw new Exception($"Could not find teacher option containing '{teacherName}'");
        // }
        //
        // await teacherDropdown.SelectOptionAsync(new[] { optionValue });
        // System.Console.WriteLine($"  [ClassesPage] Selected teacher option in {sw.ElapsedMilliseconds}ms");

        var assignButton = _page.Locator("button:has-text('Assign')");
        // click the assign button
        await assignButton.ClickAsync(new LocatorClickOptions { Force = true });
        System.Console.WriteLine($"  [ClassesPage] Clicked Assign button in {sw.ElapsedMilliseconds}ms");
        sw.Stop();
        System.Console.WriteLine($"  [ClassesPage] Teacher selection completed in {sw.ElapsedMilliseconds}ms");
    }

        public async Task SubmitAssignTeacherForm()
        {
            System.Console.WriteLine("  [ClassesPage] Submitting assign teacher form");
            var sw = Stopwatch.StartNew();
            
            var assignTeacherModal = _page.Locator("div.modal-content:has(div.modal-header:has-text('Assign Teacher'))");
            var assignButton = assignTeacherModal.Locator("button:has-text('Assign')");
            System.Console.WriteLine($"  [ClassesPage] Found Assign button in {sw.ElapsedMilliseconds}ms");
            
            await assignButton.ClickAsync(new LocatorClickOptions { Force = true });
            System.Console.WriteLine($"  [ClassesPage] Clicked Assign button in {sw.ElapsedMilliseconds}ms");
            
            try {
                var modalCloseSw = Stopwatch.StartNew();
                await _page.WaitForSelectorAsync("#assignTeacherModal:not(.show)", new PageWaitForSelectorOptions { 
                    State = WaitForSelectorState.Visible, 
                    Timeout = 10000 
                });
                modalCloseSw.Stop();
                System.Console.WriteLine($"  [ClassesPage] Modal closed in {modalCloseSw.ElapsedMilliseconds}ms");
            } catch (Exception ex) {
                System.Console.WriteLine($"  [ClassesPage] Modal did not close: {ex.Message}");
                System.Console.WriteLine($"  [ClassesPage] Attempting to force close modal after {sw.ElapsedMilliseconds}ms");
                
                await _page.EvaluateAsync("document.querySelector('#assignTeacherModal.show .btn-close')?.click()");
                System.Console.WriteLine($"  [ClassesPage] Clicked close button via JS in {sw.ElapsedMilliseconds}ms");
                
                await Task.Delay(1000);
                System.Console.WriteLine($"  [ClassesPage] Waited 1000ms after JS click");
                
                await _page.Keyboard.PressAsync("Escape");
                System.Console.WriteLine($"  [ClassesPage] Pressed Escape key in {sw.ElapsedMilliseconds}ms");
                
                await Task.Delay(1000);
                System.Console.WriteLine($"  [ClassesPage] Waited 1000ms after Escape key");
            }
            
            var networkSw = Stopwatch.StartNew();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            networkSw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Waited for network idle in {networkSw.ElapsedMilliseconds}ms");
            
            sw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Form submission completed in {sw.ElapsedMilliseconds}ms");
        }

        public async Task AssignTeacherToClass(int index, string className)
        {
            System.Console.WriteLine($"  [ClassesPage] Starting to assign teacher '{index}' to class '{className}'");
            var totalSw = Stopwatch.StartNew();
            
            await SelectTeacherFromDropdown(index);
            
            // click esc to close modal
            await _page.Keyboard.PressAsync("Escape");
            // wait 500 ms
            await Task.Delay(500);
            
            totalSw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Teacher assignment completed in {totalSw.ElapsedMilliseconds}ms");
        }

        public async Task ClickAssignStudentButton(string className)
        {
            System.Console.WriteLine("  [ClassesPage] Clicking Assign Student button");
            var sw = Stopwatch.StartNew();
            
            var classRow = _page.Locator("tr", new() { HasText = className });
            System.Console.WriteLine($"  [ClassesPage] Found class row in {sw.ElapsedMilliseconds}ms");
            
            await classRow.Locator("button:has-text('Assign Student')").ClickAsync(new LocatorClickOptions { Force = true });
            System.Console.WriteLine($"  [ClassesPage] Clicked Assign Student button in {sw.ElapsedMilliseconds}ms");
            
            await _page.WaitForSelectorAsync("div.modal-header:has-text('Assign Student')");
            System.Console.WriteLine($"  [ClassesPage] Modal appeared in {sw.ElapsedMilliseconds}ms");
            
            sw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Assign Student button click completed in {sw.ElapsedMilliseconds}ms");
        }

        public async Task SelectStudentFromDropdown(string studentName)
        {
            System.Console.WriteLine("  [ClassesPage] Selecting student from dropdown");
            var sw = Stopwatch.StartNew();
            
            var assignStudentModal = _page.Locator("div.modal-content:has(div.modal-header:has-text('Assign Student'))");
            System.Console.WriteLine($"  [ClassesPage] Found modal in {sw.ElapsedMilliseconds}ms");
            
            // Use specific ID selector for student dropdown instead of generic "select"
            var studentDropdown = assignStudentModal.Locator("#studentId");
            System.Console.WriteLine($"  [ClassesPage] Found student dropdown in {sw.ElapsedMilliseconds}ms");
            
            // Get all options and find one that contains the student name (not exact match)
            var options = await studentDropdown.Locator("option").AllAsync();
            System.Console.WriteLine($"  [ClassesPage] Found {options.Count} options in {sw.ElapsedMilliseconds}ms");
            
            string optionValue = null;
            foreach (var option in options)
            {
                var text = await option.TextContentAsync();
                if (text != null && text.Contains(studentName, StringComparison.OrdinalIgnoreCase))
                {
                    optionValue = await option.GetAttributeAsync("value");
                    System.Console.WriteLine($"  [ClassesPage] Found matching option with value: {optionValue} in {sw.ElapsedMilliseconds}ms");
                    break;
                }
            }
            
            if (optionValue == null)
            {
                throw new Exception($"Could not find student option containing '{studentName}'");
            }
            
            await studentDropdown.SelectOptionAsync(new[] { optionValue });
            System.Console.WriteLine($"  [ClassesPage] Selected student option in {sw.ElapsedMilliseconds}ms");
            
            sw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Student selection completed in {sw.ElapsedMilliseconds}ms");
        }

        public async Task SubmitAssignStudentForm()
        {
            System.Console.WriteLine("  [ClassesPage] Submitting assign student form");
            var sw = Stopwatch.StartNew();
            
            var assignStudentModal = _page.Locator("div.modal-content:has(div.modal-header:has-text('Assign Student'))");
            var assignButton = assignStudentModal.Locator("button:has-text('Assign')");
            System.Console.WriteLine($"  [ClassesPage] Found Assign button in {sw.ElapsedMilliseconds}ms");
            
            await assignButton.ClickAsync(new LocatorClickOptions { Force = true });
            System.Console.WriteLine($"  [ClassesPage] Clicked Assign button in {sw.ElapsedMilliseconds}ms");
            
            try {
                var modalCloseSw = Stopwatch.StartNew();
                await _page.WaitForSelectorAsync("#assignStudentModal:not(.show)", new PageWaitForSelectorOptions { 
                    State = WaitForSelectorState.Visible, 
                    Timeout = 10000 
                });
                modalCloseSw.Stop();
                System.Console.WriteLine($"  [ClassesPage] Modal closed in {modalCloseSw.ElapsedMilliseconds}ms");
            } catch (Exception ex) {
                System.Console.WriteLine($"  [ClassesPage] Modal did not close: {ex.Message}");
                System.Console.WriteLine($"  [ClassesPage] Attempting to force close modal after {sw.ElapsedMilliseconds}ms");
                
                await _page.EvaluateAsync("document.querySelector('#assignStudentModal.show .btn-close')?.click()");
                System.Console.WriteLine($"  [ClassesPage] Clicked close button via JS in {sw.ElapsedMilliseconds}ms");
                
                await Task.Delay(1000);
                System.Console.WriteLine($"  [ClassesPage] Waited 1000ms after JS click");
                
                await _page.Keyboard.PressAsync("Escape");
                System.Console.WriteLine($"  [ClassesPage] Pressed Escape key in {sw.ElapsedMilliseconds}ms");
                
                await Task.Delay(1000);
                System.Console.WriteLine($"  [ClassesPage] Waited 1000ms after Escape key");
            }
            
            var networkSw = Stopwatch.StartNew();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            networkSw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Waited for network idle in {networkSw.ElapsedMilliseconds}ms");
            
            sw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Form submission completed in {sw.ElapsedMilliseconds}ms");
        }

        public async Task AssignStudentToClass(string studentName, string className)
        {
            System.Console.WriteLine($"  [ClassesPage] Starting to assign student '{studentName}' to class '{className}'");
            var totalSw = Stopwatch.StartNew();
            
            await NavigateToClassesPage();
            await ClickAssignStudentButton(className);
            await SelectStudentFromDropdown(studentName);
            await SubmitAssignStudentForm();
            
            totalSw.Stop();
            System.Console.WriteLine($"  [ClassesPage] Student assignment completed in {totalSw.ElapsedMilliseconds}ms");
        }
    }
}
