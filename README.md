# Redis Lock Feature Demo

This is a demo redis lock feature, using [RedLock.net](https://github.com/redlock/redlock.net).

## How to Run the Program

1. Update the configuration in `appsettings.json`.
2. Run the command: dotnet run

## Test the Lock with Retry Feature

1. Open your web browser and test the following endpoints:

   - Set resource with retry (value = v1):
     ```
     http://localhost:5124/api/lock/set-resource-with-retry?value=v1
     ```
   - Set resource with retry (value = v2):
     ```
     http://localhost:5124/api/lock/set-resource-with-retry?value=v2
     ```
   - Get resource:
     ```
     http://localhost:5124/api/lock/get-resource
     ```

## Test the Lock with Giving Up

1. Open your web browser and test the following endpoints:

   - Set resource with giving up (value = r1):
     ```
     http://localhost:5124/api/lock/set-resource-with-giving-up?value=r1
     ```
   - Set resource with giving up (value = r2):
     ```
     http://localhost:5124/api/lock/set-resource-with-giving-up?value=r2
     ```
   - Get resource:
     ```
     http://localhost:5124/api/lock/get-resource
     ```