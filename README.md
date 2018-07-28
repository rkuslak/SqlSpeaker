SqlSpeaker - A simple TSQL query helper class
===

This is a simple "helper" class I wrote while playing around with some ideas I had for a personal project. It can take a arbitrary string and "split" it into batches similar to SQLCMD. The end result is a List of query strings which the end consumer could then send to an SQL server instance.

Eventual goals I have for this is to 'generalize' the logic more, and to perhaps revert previous behavior and have it retain comments in the split queries (for ease of debugging, it currently removes comments) and ensuring we can more this to a .Net Standard library (currently it targets Framework 4.6, if I recall correctly). Additionally, this currently creates a "test" program and runs it instead of just compiling to a dll; I would like to move this into a unit test and have it generate a actual class libraray instead. More "simple" projects to add to the pile. I would also like to recreate it as a Golang library at some point as well.

The code is licensed under the GPL v2.0.