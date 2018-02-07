## Can't Stop Driving

##### Procedure to set up your workspace :
1. Install Unity and Git on your computer (obviously).
2. Go inside an empty directory and open a terminal there.
3. Type these commands (replace branchName by the name of your own branch) :

~~~~
git config --global user.name "Firstname LASTNAME"
git config --global user.email "firstname.lastname@epita.fr"

git init

git remote add origin https://github.com/AllOfTheAbove/CantStopDriving.git

git pull origin branchName
git checkout branchName
~~~~

##### How to work on the project :
1. Open a terminal in the project directory.
2. Use these commands (replace branchName by the name of your own branch) :

~~~~
git add *
git commit -m "Describe what you've done..."
git push origin branchName
~~~~