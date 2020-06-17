/* ����ͷ�ļ� */

#include "zigzag.h"

#include "stdefine.h"

#include <string.h>

#include <stdio.h>

#include <stdlib.h>

#include <sys/types.h>

#include <sys/stat.h>

#include <fcntl.h>



/* �ڲ�ȫ�ֱ������� */

const int ZIGZAG[64] =

{

     0,  1,  8, 16,  9,  2,  3, 10,

    17, 24, 32, 25, 18, 11,  4,  5,

    12, 19, 26, 33, 40, 48, 41, 34,

    27, 20, 13,  6,  7, 14, 21, 28,

    35, 42, 49, 56, 57, 50, 43, 36,

    29, 22, 15, 23, 30, 37, 44, 51,

    58, 59, 52, 45, 38, 31, 39, 46,

    53, 60, 61, 54, 47, 55, 62, 63,

};



/* ����ʵ�� */

void zigzag_encode(int *data)

{

    int buf[64], i;
    for (i=0; i<64; i++) buf [i] = data[ZIGZAG[i]];
    for (i=0; i<64; i++) data[i] = buf[i];

}



void zigzag_decode(int *data)

{

    int buf[64], i;
	  //int test[4]={0,10,20,30};
    int zz_64_fd = open("/dev/zz_4", O_RDWR);

    //int dct_value;

    if (zz_64_fd < 0)
    {

        printf("[ERROR] can't open de-zigzag ip");

    }

    for(i=0;i<64;i++){

	     write(zz_64_fd, &data[i], i);//0610

    }

    for( i=0;i<64;i++){
      read(zz_64_fd, &data[i], i);//0610
    }
  // for(i=0;i<4;i++)printf("\n%d\n",*(test+i));

  //soft
/*
  for (i=0; i<64; i++) buf [ZIGZAG[i]] = data[i];
  for (i=0; i<64; i++) data[i] = buf[i];*/



    printf("from zigzag.c zigzag_decode ok\n");









   /* for (i=0; i<64; i++) buf [ZIGZAG[i]] = data[i];

    for (i=0; i<64; i++) data[i] = buf[i];*/

	close(zz_64_fd);

}
