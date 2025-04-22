import 'package:flutter/material.dart';

class StudentList extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Student List'),
      ),
      body: ListView(
        children: [
          StudentCard(
            studentName: 'Emma Thompson',
            grade: '5th Grade',
            pickup: '789 Pine Road',
            dropoff: 'Lincoln Elementary School',
            status: 'Active',
          ),
          StudentCard(
            studentName: 'Noah Garcia',
            grade: '3th Grade',
            pickup: '456 Oak Avenue',
            dropoff: 'Lincoln Elementary School',
            status: 'Active',
          ),
          StudentCard(
            studentName: 'Olivia Wilson',
            grade: '4th Grade',
            pickup: '101 Cedar Lane',
            dropoff: 'Lincoln Elementary School',
            status: 'Absent',
          ),
          StudentCard(
            studentName: 'Emma Thompson',
            grade: '5th Grade',
            pickup: '123 Maple Street',
            dropoff: 'Lincoln Elementary School',
            status: 'Active',
          ),
        ],
      ),
    );
  }
}

class StudentCard extends StatelessWidget {
  final String studentName;
  final String grade;
  final String pickup;
  final String dropoff;
  final String status;

  StudentCard({
    required this.studentName,
    required this.grade,
    required this.pickup,
    required this.dropoff,
    required this.status,
  });

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: EdgeInsets.all(8.0),
      child: Padding(
        padding: const EdgeInsets.all(8.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                CircleAvatar(radius: 10, backgroundColor: Colors.grey),
                SizedBox(width: 8),
                Text(studentName, style: TextStyle(fontWeight: FontWeight.bold)),
              ],
            ),
            Text(grade),
            SizedBox(height: 8),
            Text('Pickup: $pickup'),
            Text('Dropoff: $dropoff'),
            SizedBox(height: 8),
            Text('Status: $status'),
            ElevatedButton(
              onPressed: () {},
              child: Text('Notify Parent'),
            ),
          ],
        ),
      ),
    );
  }
}
