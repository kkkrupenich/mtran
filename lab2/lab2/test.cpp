#include <iostream>

using namespace std;

void Caesar()
{
    int key=0;
    string str="";

    cout<<"Enter string to encode: ";
    cin>>str;

    cout<< "Enter key: ";
    cin >>key;  

    cout << "String to encode: " << str << endl;
    key%= 26;

    for (int i =0;i<str.length();i++)
    {
        if (str[i]+key>'z') 
            str[i]-=26;
        str[i] += key;     
    }
    
    cout << "Encoded string: " << str << endl;

    for (int i = 0; i < str.length(); i++)
    {
        if (str[i] - key < 'a')
            str[i] += 26;
        str[i] -= key;
    }

    cout << "Decoded string: " << str << endl << endl;
}

int main()
{
    Caesar();
    return 0;
}
