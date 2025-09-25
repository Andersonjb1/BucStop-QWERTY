# Snake Game Documentation

## Current Status

The Snake game is functional when users interact with it, but several issues have been identified.

## Known Issues

### Gameplay Bugs
1. **Speed Control**: Game speeds up when hitting the space bar
2. **Wall Collision**: Snake won't crash into walls as expected
3. **Screen Movement**: Screen moves whenever arrow keys are pressed

## Technical Implementation

### Touch Screen Support
- **Hammer.min.js**: Interactive API that enables touchscreen users to interact with the game

### Service Integration
- **program.cs**: Publishes a JSON endpoint for services

## Code Structure Observations

### Duplicate Implementation
- A copy of Snake exists within BucStop
- The copy serves as a deprecated page that loads before the actual game using JS files

### Mystery Implementation
The Snake game continues to function even after attempting to delete code traces. Despite removing as much code as possible, the exact mechanism keeping the game operational remains unclear.

## Areas for Investigation

1. **Root Cause Analysis**: Determine why the game continues to work despite code deletion
2. **Bug Fixes**: Address the speed, collision, and screen movement issues
3. **Code Cleanup**: Identify and remove duplicate/deprecated implementations
4. **Architecture Review**: Understand the actual game loading mechanism
