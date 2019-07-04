For /f "tokens=2-4 delims=/ " %%a in ('date /t') do (set _date=%%c_%%a_%%b)
For /f "tokens=1-2 delims=/:" %%a in ("%TIME: =0%") do (set _time=%%a%%b)
dotnet build
dotnet ef migrations add V%_date%_%_time%  --output-dir Infrastructure/Migrations
pause