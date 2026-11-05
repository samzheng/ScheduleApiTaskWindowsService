# ScheduleApiTaskWindowsService
config schedued api task in a windows service

帮我用vs2013, .net framework 4.5写一个windows service，里面我可以用配置文件配置一些定时运行的任务，任务可以是每天几点几分几秒运行，或者每月哪一天几点几分几秒运行，或者是每几分钟运行一次。 
 至于实际的任务，是访问一个网址api接口，有get也有post，url可以配置。 
 每一次的任务，都要写单独的日志，调用前，和调用结果都要写到这个日期，和任务名称结合的日志文件里，如 2026-11-06_ResetAssignment.log，日志里每条日志要加时间，后加tab，然后是日志内容。时间格式为HH:mm:ss. 
 api接口返回的内容，要记录在日志内容里。 
 这个解决方案就用ScheduledApiTaskWindowsService


解决方案已完成，编译通过，生成了 `bin\Release\ScheduledApiTaskWindowsService.exe`。代码全部按 VS2013 / .NET 4.5 / C# 5 语法编写，可直接用 VS2013 打开 [ScheduledApiTaskWindowsService.sln](file:///c:/Users/Sam/Desktop/ScheduleTaskWindowsService/ScheduledApiTaskWindowsService.sln)。

## 结构

- [ScheduledTaskService.cs](file:///c:/Users/Sam/Desktop/ScheduleTaskWindowsService/ScheduledApiTaskWindowsService/ScheduledTaskService.cs) — 服务主体（启动时启用 TLS 1.1/1.2 兼容）
- [TaskScheduler.cs](file:///c:/Users/Sam/Desktop/ScheduleTaskWindowsService/ScheduledApiTaskWindowsService/Core/TaskScheduler.cs) — 调度器，每秒检查一次，到期任务放线程池执行，同一任务不会并发重入
- [ScheduledTask.cs](file:///c:/Users/Sam/Desktop/ScheduleTaskWindowsService/ScheduledApiTaskWindowsService/Core/ScheduledTask.cs) — 到期判断（Daily / Monthly / Interval）
- [HttpHelper.cs](file:///c:/Users/Sam/Desktop/ScheduleTaskWindowsService/ScheduledApiTaskWindowsService/Core/HttpHelper.cs) — GET/POST 调用，记录状态码与返回内容
- [TaskLogger.cs](file:///c:/Users/Sam/Desktop/ScheduleTaskWindowsService/ScheduledApiTaskWindowsService/Core/TaskLogger.cs) — 按 `yyyy-MM-dd_任务名.log` 写日志，每行 `HH:mm:ss` + Tab + 内容
- [App.config](file:///c:/Users/Sam/Desktop/ScheduleTaskWindowsService/ScheduledApiTaskWindowsService/App.config) — 任务配置，含三个示例

## 配置示例

```xml
<task name="ResetAssignment" url="http://localhost:8080/api/reset" method="POST"
      postData="{&quot;mode&quot;:&quot;daily&quot;}" scheduleType="Daily" timeOfDay="02:00:00" />
<task name="SyncData" url="http://localhost:8080/api/sync" method="GET"
      scheduleType="Interval" intervalMinutes="5" />
<task name="MonthlyReport" url="http://localhost:8080/api/report" method="POST"
      scheduleType="Monthly" dayOfMonth="1" timeOfDay="03:30:00" />
```

- `Daily`：每天 `timeOfDay`（HH:mm:ss）运行
- `Monthly`：每月 `dayOfMonth` 日 `timeOfDay` 运行（如配 31 日而当月只有 30 天，则取当月最后一天）
- `Interval`：每 `intervalMinutes` 分钟运行一次，服务启动后立即执行第一次

日志写在 exe 目录下的 `Logs\` 文件夹，如 `Logs\2026-11-06_ResetAssignment.log`：

```
14:30:00	开始调用 POST http://localhost:8080/api/reset
14:30:01	调用成功，状态码：200，返回内容：{"code":0,"message":"ok"}
```

## 安装 / 卸载（管理员命令行）

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe ScheduledApiTaskWindowsService.exe
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe /u ScheduledApiTaskWindowsService.exe
```

修改任务配置只需编辑 `ScheduledApiTaskWindowsService.exe.config` 后重启服务。