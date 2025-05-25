#!/usr/bin/env python3
from pytm.pytm import TM, Server, Datastore, Dataflow, Actor

tm = TM("User Login TM")

tm.isOrdered = True

user = Actor("User")
web = Server("Web Server")
db = Datastore("MySQL Database (*)")

user_to_web = Dataflow(user, web, "User enters credentials")
web_to_user = Dataflow(web, user, "Login success or failure")
web_to_db = Dataflow(web, db, "Query user credentials")
db_to_web = Dataflow(db, web, "User information response")

tm.process()
